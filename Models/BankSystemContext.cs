using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API.Models;
public class BankSystemContext: DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryGoal> CategoryGoals => Set<CategoryGoal>();
    public DbSet<TransactionSplit> TranactionSplits => Set<TransactionSplit>();
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
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<CategoryGoal>(entity =>
        {
            entity.ToTable("CategoryGoals");
            entity.HasOne(goal => goal.Category) 
                  .WithOne(cat => cat.CategoryGoal)
                  .HasForeignKey<CategoryGoal>(goal => goal.CategoryId);
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<TransactionSplit>(entity =>
        {
            entity.ToTable("TransactionSplits");
            // Make sure this is correct
            entity.HasOne(cat => cat.Category) 
                  .WithOne(ts => ts.TransactionSplit)
                  .HasForeignKey<TransactionSplit>(cat => cat.CategoryId);
            entity.HasOne(trans => trans.Transaction) 
                  .WithOne(ts => ts.TransactionSplit)
                  .HasForeignKey<TransactionSplit>(goal => goal.TransactionId);
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}