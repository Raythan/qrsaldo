using Microsoft.EntityFrameworkCore;
using QRSaldo.API.Data;
using QRSaldo.API.DTOs;
using QRSaldo.API.Models;

namespace QRSaldo.API.Services
{
    public interface IUsuarioService
    {
        Task<ResultadoOperacao<UsuarioDto>> CriarUsuarioAsync(CriarUsuarioDto dto);
        Task<ResultadoOperacao<UsuarioDto>> ObterUsuarioPorTelefoneAsync(string telefone);
        Task<ResultadoOperacao<UsuarioDto>> ObterUsuarioPorIdAsync(Guid id);
        Task<ResultadoOperacao<List<TransacaoDto>>> ObterHistoricoTransacoesAsync(Guid usuarioId);
        Task<ResultadoOperacao<List<PedidoDto>>> ObterPedidosUsuarioAsync(Guid usuarioId);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly QRSaldoContext _context;

        public UsuarioService(QRSaldoContext context)
        {
            _context = context;
        }

        public async Task<ResultadoOperacao<UsuarioDto>> CriarUsuarioAsync(CriarUsuarioDto dto)
        {
            try
            {
                // Verificar se já existe usuário com este telefone
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Telefone == dto.Telefone);

                if (usuarioExistente != null)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Já existe um usuário com este telefone",
                        Erros = new List<string> { "Telefone já cadastrado" }
                    };
                }

                var usuario = new Usuario
                {
                    Nome = dto.Nome,
                    Telefone = dto.Telefone
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

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
                    Mensagem = "Usuário criado com sucesso",
                    Dados = usuarioDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<UsuarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<UsuarioDto>> ObterUsuarioPorTelefoneAsync(string telefone)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Telefone == telefone);

                if (usuario == null)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado",
                        Erros = new List<string> { "Telefone não cadastrado" }
                    };
                }

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
                    Mensagem = "Usuário encontrado",
                    Dados = usuarioDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<UsuarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<UsuarioDto>> ObterUsuarioPorIdAsync(Guid id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return new ResultadoOperacao<UsuarioDto>
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado"
                    };
                }

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
                    Mensagem = "Usuário encontrado",
                    Dados = usuarioDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<UsuarioDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<List<TransacaoDto>>> ObterHistoricoTransacoesAsync(Guid usuarioId)
        {
            try
            {
                var transacoes = await _context.Transacoes
                    .Where(t => t.UsuarioId == usuarioId)
                    .OrderByDescending(t => t.DataHora)
                    .Select(t => new TransacaoDto
                    {
                        Id = t.Id,
                        Tipo = t.Tipo.ToString(),
                        Valor = t.Valor,
                        SaldoAnterior = t.SaldoAnterior,
                        SaldoNovo = t.SaldoNovo,
                        Descricao = t.Descricao,
                        DataHora = t.DataHora
                    })
                    .ToListAsync();

                return new ResultadoOperacao<List<TransacaoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Histórico obtido com sucesso",
                    Dados = transacoes
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<List<TransacaoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<List<PedidoDto>>> ObterPedidosUsuarioAsync(Guid usuarioId)
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Barraca)
                    .Include(p => p.Produto)
                    .Where(p => p.UsuarioId == usuarioId)
                    .OrderByDescending(p => p.DataHora)
                    .Select(p => new PedidoDto
                    {
                        Id = p.Id,
                        NumeroPedido = p.NumeroPedido,
                        UsuarioNome = p.Usuario.Nome,
                        UsuarioTelefone = p.Usuario.Telefone,
                        BarracaNome = p.Barraca.Nome,
                        ProdutoNome = p.Produto.Nome,
                        Quantidade = p.Quantidade,
                        ValorUnitario = p.ValorUnitario,
                        ValorTotal = p.ValorTotal,
                        Status = p.Status.ToString(),
                        Observacoes = p.Observacoes,
                        DataHora = p.DataHora,
                        DataEntrega = p.DataEntrega
                    })
                    .ToListAsync();

                return new ResultadoOperacao<List<PedidoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Pedidos obtidos com sucesso",
                    Dados = pedidos
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<List<PedidoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }
    }
}
