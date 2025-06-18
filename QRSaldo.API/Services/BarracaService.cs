using Microsoft.EntityFrameworkCore;
using QRSaldo.API.Data;
using QRSaldo.API.DTOs;
using QRSaldo.API.Models;

namespace QRSaldo.API.Services
{
    public interface IBarracaService
    {
        Task<ResultadoOperacao<List<BarracaDto>>> ObterBarracasAsync();
        Task<ResultadoOperacao<BarracaDto>> ObterBarracaPorIdAsync(int id);
        Task<ResultadoOperacao<List<ProdutoDto>>> ObterProdutosPorBarracaAsync(int barracaId);
        Task<ResultadoOperacao<List<PedidoDto>>> ObterPedidosPorBarracaAsync(int barracaId, StatusPedido? status = null);
        Task<ResultadoOperacao<PedidoDto>> AtualizarStatusPedidoAsync(Guid pedidoId, AtualizarStatusPedidoDto dto);
    }

    public class BarracaService : IBarracaService
    {
        private readonly QRSaldoContext _context;

        public BarracaService(QRSaldoContext context)
        {
            _context = context;
        }        public async Task<ResultadoOperacao<List<BarracaDto>>> ObterBarracasAsync()
        {
            try
            {
                var barracas = await _context.Barracas
                    .Where(b => b.Ativa)
                    .ToListAsync();

                var barracasDto = new List<BarracaDto>();
                
                foreach (var barraca in barracas)
                {
                    var produtos = await _context.Produtos
                        .Where(p => p.BarracaId == barraca.Id && p.Ativo)
                        .ToListAsync();

                    barracasDto.Add(new BarracaDto
                    {
                        Id = barraca.Id,
                        Nome = barraca.Nome,
                        Descricao = barraca.Descricao,
                        Ativa = barraca.Ativa,
                        Produtos = produtos.Select(p => new ProdutoDto
                        {
                            Id = p.Id,
                            Nome = p.Nome,
                            Descricao = p.Descricao,
                            Preco = p.Preco,
                            Ativo = p.Ativo,
                            BarracaId = p.BarracaId,
                            BarracaNome = barraca.Nome
                        }).ToList()
                    });
                }                return new ResultadoOperacao<List<BarracaDto>>
                {
                    Sucesso = true,
                    Mensagem = "Barracas obtidas com sucesso",
                    Dados = barracasDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<List<BarracaDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }        public async Task<ResultadoOperacao<BarracaDto>> ObterBarracaPorIdAsync(int id)
        {
            try
            {
                var barraca = await _context.Barracas
                    .Where(b => b.Id == id)
                    .FirstOrDefaultAsync();

                if (barraca == null)
                {
                    return new ResultadoOperacao<BarracaDto>
                    {
                        Sucesso = false,
                        Mensagem = "Barraca não encontrada"
                    };
                }

                var produtos = await _context.Produtos
                    .Where(p => p.BarracaId == barraca.Id && p.Ativo)
                    .ToListAsync();

                var barracaDto = new BarracaDto
                {
                    Id = barraca.Id,
                    Nome = barraca.Nome,
                    Descricao = barraca.Descricao,
                    Ativa = barraca.Ativa,
                    Produtos = produtos.Select(p => new ProdutoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Preco = p.Preco,
                        Ativo = p.Ativo,
                        BarracaId = p.BarracaId,
                        BarracaNome = barraca.Nome
                    }).ToList()
                };

                return new ResultadoOperacao<BarracaDto>
                {
                    Sucesso = true,
                    Mensagem = "Barraca encontrada",
                    Dados = barracaDto
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<BarracaDto>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<List<ProdutoDto>>> ObterProdutosPorBarracaAsync(int barracaId)
        {
            try
            {
                var produtos = await _context.Produtos
                    .Include(p => p.Barraca)
                    .Where(p => p.BarracaId == barracaId && p.Ativo && p.Barraca.Ativa)
                    .Select(p => new ProdutoDto
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        Descricao = p.Descricao,
                        Preco = p.Preco,
                        Ativo = p.Ativo,
                        BarracaId = p.BarracaId,
                        BarracaNome = p.Barraca.Nome
                    })
                    .ToListAsync();

                return new ResultadoOperacao<List<ProdutoDto>>
                {
                    Sucesso = true,
                    Mensagem = "Produtos obtidos com sucesso",
                    Dados = produtos
                };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<List<ProdutoDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor",
                    Erros = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultadoOperacao<List<PedidoDto>>> ObterPedidosPorBarracaAsync(int barracaId, StatusPedido? status = null)
        {
            try
            {
                var query = _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Barraca)
                    .Include(p => p.Produto)
                    .Where(p => p.BarracaId == barracaId);

                if (status.HasValue)
                {
                    query = query.Where(p => p.Status == status.Value);
                }

                var pedidos = await query
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

        public async Task<ResultadoOperacao<PedidoDto>> AtualizarStatusPedidoAsync(Guid pedidoId, AtualizarStatusPedidoDto dto)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .Include(p => p.Barraca)
                    .Include(p => p.Produto)
                    .FirstOrDefaultAsync(p => p.Id == pedidoId);

                if (pedido == null)
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Pedido não encontrado"
                    };
                }

                if (!Enum.TryParse<StatusPedido>(dto.Status, true, out var novoStatus))
                {
                    return new ResultadoOperacao<PedidoDto>
                    {
                        Sucesso = false,
                        Mensagem = "Status inválido",
                        Erros = new List<string> { "Status deve ser: Pendente, Preparando, Pronto, Entregue ou Cancelado" }
                    };
                }

                pedido.Status = novoStatus;
                
                if (novoStatus == StatusPedido.Entregue)
                {
                    pedido.DataEntrega = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                var pedidoDto = new PedidoDto
                {
                    Id = pedido.Id,
                    NumeroPedido = pedido.NumeroPedido,
                    UsuarioNome = pedido.Usuario.Nome,
                    UsuarioTelefone = pedido.Usuario.Telefone,
                    BarracaNome = pedido.Barraca.Nome,
                    ProdutoNome = pedido.Produto.Nome,
                    Quantidade = pedido.Quantidade,
                    ValorUnitario = pedido.ValorUnitario,
                    ValorTotal = pedido.ValorTotal,
                    Status = pedido.Status.ToString(),
                    Observacoes = pedido.Observacoes,
                    DataHora = pedido.DataHora,
                    DataEntrega = pedido.DataEntrega
                };

                return new ResultadoOperacao<PedidoDto>
                {
                    Sucesso = true,
                    Mensagem = $"Status do pedido #{pedido.NumeroPedido} atualizado para {novoStatus}",
                    Dados = pedidoDto
                };
            }
            catch (Exception ex)
            {
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
