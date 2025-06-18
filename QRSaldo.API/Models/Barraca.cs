using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public class Barraca
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Descricao { get; set; }
        
        public bool Ativa { get; set; } = true;
        
        public DateTime CriadoEm { get; set; } = DateTime.Now;
        
        // Navegação
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
