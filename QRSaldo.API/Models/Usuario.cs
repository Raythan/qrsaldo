using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(15)]
        public string Telefone { get; set; } = string.Empty;
        
        public decimal Saldo { get; set; } = 0;
        
        public DateTime UltimaAtualizacao { get; set; } = DateTime.Now;
        
        public DateTime CriadoEm { get; set; } = DateTime.Now;
        
        // Navegação
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
