using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public enum StatusPedido
    {
        Pendente = 1,
        Preparando = 2,
        Pronto = 3,
        Entregue = 4,
        Cancelado = 5
    }

    public class Pedido
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public int NumeroPedido { get; set; }
        
        public Guid UsuarioId { get; set; }
        
        public int BarracaId { get; set; }
        
        public int ProdutoId { get; set; }
        
        public int Quantidade { get; set; } = 1;
        
        public decimal ValorUnitario { get; set; }
        
        public decimal ValorTotal { get; set; }
        
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        
        [MaxLength(500)]
        public string? Observacoes { get; set; }
        
        public DateTime DataHora { get; set; } = DateTime.Now;
        
        public DateTime? DataEntrega { get; set; }
        
        // Navegação
        public Usuario Usuario { get; set; } = null!;
        public Barraca Barraca { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}
