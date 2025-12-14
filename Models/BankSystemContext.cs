using Microsoft.EntityFrameworkCore;

namespace BankSystem.API.Models;
public class BankSystemContext: DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public BankSystemContext(DbContextOptions<BankSystemContext> options): base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity=>
        {
            entity.ToTable("Transactions");
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}