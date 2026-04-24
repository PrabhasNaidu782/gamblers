using Microsoft.EntityFrameworkCore;
using GamblersGrocery.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using FluentValidation;

namespace GamblersGrocery.Data
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  
        public string PlainPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<StockLog> StockLogs { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(e =>
            {
                e.HasKey(u => u.UserId);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Email).HasMaxLength(200).IsRequired();
                e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
                e.Property(u => u.Role).HasMaxLength(50).IsRequired();
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.PlainPassword).HasMaxLength(200);
            });

            builder.Entity<StockLog>()
                .HasOne(s => s.Product).WithMany(p => p.StockLogs)
                .HasForeignKey(s => s.productId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Promotion>()
                .HasOne(p => p.Product).WithMany(pr => pr.Promotions)
                .HasForeignKey(p => p.productId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransactionItem>()
                .HasOne(t => t.Transaction).WithMany(tr => tr.TransactionItems)
                .HasForeignKey(t => t.transactionId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransactionItem>()
                .HasOne(t => t.Product).WithMany()
                .HasForeignKey(t => t.productId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
