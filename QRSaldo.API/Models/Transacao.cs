using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public enum TipoTransacao
    {
        Credito = 1,
        Debito = 2
    }

    public class Transacao
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UsuarioId { get; set; }
        
        public TipoTransacao Tipo { get; set; }
        
        public decimal Valor { get; set; }
        
        public decimal SaldoAnterior { get; set; }
        
        public decimal SaldoNovo { get; set; }
        
        [MaxLength(500)]
        public string? Descricao { get; set; }
        
        public Guid? PedidoId { get; set; }
        
        public Guid? TokenCreditoId { get; set; }
        
        public DateTime DataHora { get; set; } = DateTime.Now;
        
        // Navegação
        public Usuario Usuario { get; set; } = null!;
        public Pedido? Pedido { get; set; }
        public TokenCredito? TokenCredito { get; set; }
    }
}
