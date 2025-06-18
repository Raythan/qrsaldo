namespace QRSaldo.API.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }

    public class CriarUsuarioDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }

    public class BarracaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Ativa { get; set; }
        public List<ProdutoDto> Produtos { get; set; } = new();
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; }
        public int BarracaId { get; set; }
        public string? BarracaNome { get; set; }
    }

    public class PedidoDto
    {
        public Guid Id { get; set; }
        public int NumeroPedido { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string UsuarioTelefone { get; set; } = string.Empty;
        public string BarracaNome { get; set; } = string.Empty;
        public string ProdutoNome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime? DataEntrega { get; set; }
    }

    public class TransacaoDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNovo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataHora { get; set; }
    }

    public class CreditarSaldoDto
    {
        public string Token { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }

    public class ConsumirSaldoDto
    {
        public string Telefone { get; set; } = string.Empty;
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; } = 1;
        public string? Observacoes { get; set; }
    }

    public class CriarTokenCreditoDto
    {
        public decimal Valor { get; set; }
        public int ValidadePorMinutos { get; set; } = 60; // 1 hora por padrão
    }

    public class TokenCreditoDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime ExpiresEm { get; set; }
        public bool Usado { get; set; }
        public DateTime? UsadoEm { get; set; }
    }

    public class AtualizarStatusPedidoDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ResultadoOperacao<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}
