using System.ComponentModel.DataAnnotations;

namespace QRSaldo.API.Models
{
    public class TokenCredito
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;
        
        public decimal Valor { get; set; }
        
        public DateTime CriadoEm { get; set; } = DateTime.Now;
        
        public DateTime ExpiresEm { get; set; }
        
        public bool Usado { get; set; } = false;
        
        public DateTime? UsadoEm { get; set; }
        
        public Guid? UsuarioId { get; set; }
        
        [MaxLength(500)]
        public string? AssinaturaCaixa { get; set; }
        
        // Navegação
        public Usuario? Usuario { get; set; }
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
