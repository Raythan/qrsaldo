using Microsoft.EntityFrameworkCore;
using QRSaldo.API.Models;

namespace QRSaldo.API.Data
{
    public class QRSaldoContext : DbContext
    {
        public QRSaldoContext(DbContextOptions<QRSaldoContext> options) : base(options)
        {
        }        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Barraca> Barracas { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<Transacao> Transacoes { get; set; } = null!;
        public DbSet<TokenCredito> TokensCredito { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de precisão decimal
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Saldo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Transacao>()
                .Property(t => t.Valor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Transacao>()
                .Property(t => t.SaldoAnterior)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Transacao>()
                .Property(t => t.SaldoNovo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TokenCredito>()
                .Property(tc => tc.Valor)
                .HasPrecision(10, 2);

            // Relacionamentos
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Barraca)
                .WithMany(b => b.Produtos)
                .HasForeignKey(p => p.BarracaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Barraca)
                .WithMany(b => b.Pedidos)
                .HasForeignKey(p => p.BarracaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Produto)
                .WithMany(pr => pr.Pedidos)
                .HasForeignKey(p => p.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Transacoes)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.TokenCredito)
                .WithMany(tc => tc.Transacoes)
                .HasForeignKey(t => t.TokenCreditoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TokenCredito>()
                .HasOne(tc => tc.Usuario)
                .WithMany()
                .HasForeignKey(tc => tc.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Telefone)
                .IsUnique();

            modelBuilder.Entity<TokenCredito>()
                .HasIndex(tc => tc.Token)
                .IsUnique();

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.NumeroPedido)
                .IsUnique();

            // Configurações de auto-incremento para número do pedido
            modelBuilder.Entity<Pedido>()
                .Property(p => p.NumeroPedido)
                .ValueGeneratedOnAdd();

            // Dados iniciais (Seed Data)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Barracas iniciais
            modelBuilder.Entity<Barraca>().HasData(
                new Barraca { Id = 1, Nome = "Barraca do Pastel", Descricao = "Pastéis variados e caldo de cana" },
                new Barraca { Id = 2, Nome = "Doces da Vovó", Descricao = "Doces tradicionais e quentão" },
                new Barraca { Id = 3, Nome = "Churrasquinho", Descricao = "Espetinhos e linguiça" },
                new Barraca { Id = 4, Nome = "Bebidas", Descricao = "Refrigerantes, água e cerveja" }
            );

            // Produtos iniciais
            modelBuilder.Entity<Produto>().HasData(
                // Barraca do Pastel
                new Produto { Id = 1, Nome = "Pastel de Carne", Preco = 8.00m, BarracaId = 1 },
                new Produto { Id = 2, Nome = "Pastel de Queijo", Preco = 7.00m, BarracaId = 1 },
                new Produto { Id = 3, Nome = "Caldo de Cana", Preco = 5.00m, BarracaId = 1 },
                
                // Doces da Vovó
                new Produto { Id = 4, Nome = "Brigadeiro", Preco = 3.00m, BarracaId = 2 },
                new Produto { Id = 5, Nome = "Beijinho", Preco = 3.00m, BarracaId = 2 },
                new Produto { Id = 6, Nome = "Quentão", Preco = 6.00m, BarracaId = 2 },
                
                // Churrasquinho
                new Produto { Id = 7, Nome = "Espeto de Carne", Preco = 12.00m, BarracaId = 3 },
                new Produto { Id = 8, Nome = "Espeto de Frango", Preco = 10.00m, BarracaId = 3 },
                new Produto { Id = 9, Nome = "Linguiça", Preco = 8.00m, BarracaId = 3 },
                
                // Bebidas
                new Produto { Id = 10, Nome = "Refrigerante Lata", Preco = 4.00m, BarracaId = 4 },
                new Produto { Id = 11, Nome = "Água", Preco = 2.00m, BarracaId = 4 },
                new Produto { Id = 12, Nome = "Cerveja", Preco = 6.00m, BarracaId = 4 }
            );
        }
    }
}
