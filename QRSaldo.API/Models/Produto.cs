using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Descricao { get; set; }
        
        public decimal Preco { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public int BarracaId { get; set; }
        
        public DateTime CriadoEm { get; set; } = DateTime.Now;
        
        // Navegação
        public Barraca Barraca { get; set; } = null!;
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
