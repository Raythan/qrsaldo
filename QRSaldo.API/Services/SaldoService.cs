using Microsoft.EntityFrameworkCore;
using QRSaldo.API.Data;
using QRSaldo.API.DTOs;
using QRSaldo.API.Models;

namespace QRSaldo.API.Services
{
    public interface ISaldoService
    {
        Task<ResultadoOperacao<TokenCreditoDto>> CriarTokenCreditoAsync(CriarTokenCreditoDto dto);
        Task<ResultadoOperacao<UsuarioDto>> CreditarSaldoAsync(CreditarSaldoDto dto);
        Task<ResultadoOperacao<PedidoDto>> ConsumirSaldoAsync(ConsumirSaldoDto dto);
    }

    public class SaldoService : ISaldoService
    {
        private readonly QRSaldoContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUsuarioService _usuarioService;

        public SaldoService(QRSaldoContext context, ITokenService tokenService, IUsuarioService usuarioService)
        {
            _context = context;
            _tokenService = tokenService;
            _usuarioService = usuarioService;
        }

        public async Task<ResultadoOperacao<TokenCreditoDto>> CriarTokenCreditoAsync(CriarTokenCreditoDto dto)
        {
            try
            {
                if (dto.Valor <= 0)
                {
                    return new ResultadoOperacao<TokenCreditoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Valor deve ser maior que zero",
                        Erros = new List<string> { "Valor inválido" }
                    };
                }

                var expiracao = DateTime.Now.AddMinutes(dto.ValidadePorMinutos);
                var token = _tokenService.GerarToken(dto.Valor, expiracao);

                var tokenCredito = new TokenCredito
                {
                    Token = token,
                    Valor = dto.Valor,
                    ExpiresEm = expiracao,
                    AssinaturaCaixa = "CAIXA_PRINCIPAL" // Identificador do caixa
                };

                _context.TokensCredito.Add(tokenCredito);
                await _context.SaveChangesAsync();

                var tokenDto = new TokenCreditoDto
                {
                    Id = tokenCredito.Id,
                    Token = tokenCredito.Token,
                    Valor = tokenCredito.Valor,
                    CriadoEm = tokenCredito.CriadoEm,
                    ExpiresEm = tokenCredito.ExpiresEm,
                    Usado = tokenCredito.Usado,
                    UsadoEm = tokenCredito.UsadoEm
                };

                return new ResultadoOperacao<TokenCreditoDto>
                {
                    Sucesso = true,
                    Mensagem = "Token de crédito criado com sucesso",
                    Dados = tokenDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<TokenCreditoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<UsuarioDto>> CreditarSaldoAsync(CreditarSaldoDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Buscar token no banco
                var tokenCredito = await _context.TokensCredito
                    .FirstOrDefaultAsync(tc => tc.Token == dto.Token);

                if (tokenCredito == null)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Token inválido",
                        Erros = new List<string> { "Token não encontrado" }
                    };
                }

                if (tokenCredito.Usado)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Token já foi utilizado",
                        Erros = new List<string> { "Token expirado" }
                    };
                }

                if (tokenCredito.ExpiresEm < DateTime.Now)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Token expirado",
                        Erros = new List<string> { "Token fora da validade" }
                    };
                }

                // Validar token
                if (!_tokenService.ValidarToken(dto.Token, tokenCredito.Valor, out _))
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Token inválido",
                        Erros = new List<string> { "Assinatura do token inválida" }
                    };
                }

                // Buscar usuário por telefone
                var resultadoUsuario = await _usuarioService.ObterUsuarioPorTelefoneAsync(dto.Telefone);
                
                Usuario usuario;
                if (!resultadoUsuario.Sucesso)
                {
                    // Criar usuário automaticamente se não existir
                    var novoUsuario = new Usuario
                    {
                        Nome = $"Usuário {dto.Telefone}",
                        Telefone = dto.Telefone
                    };
                    
                    _context.Usuarios.Add(novoUsuario);
                    await _context.SaveChangesAsync();
                    usuario = novoUsuario;
                }
                else
                {
                    usuario = await _context.Usuarios
                        .FirstAsync(u => u.Telefone == dto.Telefone);
                }

                // Atualizar saldo
                var saldoAnterior = usuario.Saldo;
                usuario.Saldo += tokenCredito.Valor;
                usuario.UltimaAtualizacao = DateTime.Now;

                // Marcar token como usado
                tokenCredito.Usado = true;
                tokenCredito.UsadoEm = DateTime.Now;
                tokenCredito.UsuarioId = usuario.Id;

                // Criar transação
                var transacao = new Transacao
                {
                    UsuarioId = usuario.Id,
                    Tipo = TipoTransacao.Credito,
                    Valor = tokenCredito.Valor,
                    SaldoAnterior = saldoAnterior,
                    SaldoNovo = usuario.Saldo,
                    Descricao = $"Crédito via token do caixa - Valor: R$ {tokenCredito.Valor:F2}",
                    TokenCreditoId = tokenCredito.Id
                };

                _context.Transacoes.Add(transacao);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Telefone = usuario.Telefone,
                    Saldo = usuario.Saldo,
                    UltimaAtualizacao = usuario.UltimaAtualizacao
                };

                return new ResultadoOperacao<UsuarioDto>
                {
                    Sucesso = true,
                    Mensagem = $"Saldo creditado com sucesso! Valor: R$ {tokenCredito.Valor:F2}",
                    Dados = usuarioDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResultadoOperacao<UsuarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<PedidoDto>> ConsumirSaldoAsync(ConsumirSaldoDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Buscar usuário
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Telefone == dto.Telefone);

                if (usuario == null)
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado",
                        Erros = new List<string> { "Telefone não cadastrado" }
                    };
                }

                // Buscar produto
                var produto = await _context.Produtos
                    .Include(p => p.Barraca)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProdutoId && p.Ativo);

                if (produto == null)
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Produto não encontrado ou inativo",
                        Erros = new List<string> { "Produto inválido" }
                    };
                }

                if (!produto.Barraca.Ativa)
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Barraca não está ativa",
                        Erros = new List<string> { "Barraca inativa" }
                    };
                }

                // Calcular valor total
                var valorTotal = produto.Preco * dto.Quantidade;

                // Verificar saldo
                if (usuario.Saldo < valorTotal)
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Saldo insuficiente",
                        Erros = new List<string> { $"Saldo atual: R$ {usuario.Saldo:F2}, Valor necessário: R$ {valorTotal:F2}" }
                    };
                }

                // Gerar número do pedido
                var ultimoPedido = await _context.Pedidos
                    .OrderByDescending(p => p.NumeroPedido)
                    .FirstOrDefaultAsync();
                
                var numeroPedido = (ultimoPedido?.NumeroPedido ?? 0) + 1;

                // Criar pedido
                var pedido = new Pedido
                {
                    NumeroPedido = numeroPedido,
                    UsuarioId = usuario.Id,
                    BarracaId = produto.BarracaId,
                    ProdutoId = produto.Id,
                    Quantidade = dto.Quantidade,
                    ValorUnitario = produto.Preco,
                    ValorTotal = valorTotal,
                    Observacoes = dto.Observacoes
                };

                _context.Pedidos.Add(pedido);

                // Atualizar saldo do usuário
                var saldoAnterior = usuario.Saldo;
                usuario.Saldo -= valorTotal;
                usuario.UltimaAtualizacao = DateTime.Now;

                // Criar transação
                var transacao = new Transacao
                {
                    UsuarioId = usuario.Id,
                    Tipo = TipoTransacao.Debito,
                    Valor = valorTotal,
                    SaldoAnterior = saldoAnterior,
                    SaldoNovo = usuario.Saldo,
                    Descricao = $"Compra - {produto.Nome} (Pedido #{numeroPedido})",
                    PedidoId = pedido.Id
                };

                _context.Transacoes.Add(transacao);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var pedidoDto = new PedidoDto
                {
                    Id = pedido.Id,
                    NumeroPedido = pedido.NumeroPedido,
                    UsuarioNome = usuario.Nome,
                    UsuarioTelefone = usuario.Telefone,
                    BarracaNome = produto.Barraca.Nome,
                    ProdutoNome = produto.Nome,
                    Quantidade = pedido.Quantidade,
                    ValorUnitario = pedido.ValorUnitario,
                    ValorTotal = pedido.ValorTotal,
                    Status = pedido.Status.ToString(),
                    Observacoes = pedido.Observacoes,
                    DataHora = pedido.DataHora
                };

                return new ResultadoOperacao<PedidoDto>
                {
                    Sucesso = true,
                    Mensagem = $"Pedido #{numeroPedido} criado com sucesso! Saldo restante: R$ {usuario.Saldo:F2}",
                    Dados = pedidoDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResultadoOperacao<PedidoDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}
