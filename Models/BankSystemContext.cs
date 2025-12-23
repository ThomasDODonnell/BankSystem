using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API.Models;
public class BankSystemContext: DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryGoal> CategoryGoals => Set<CategoryGoal>();
    public DbSet<TransactionSplit> TranactionSplits => Set<TransactionSplit>();
    public DbSet<TransactionCategory> TranactionCategories => Set<TransactionCategory>();
    public BankSystemContext(DbContextOptions<BankSystemContext> options): base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // CategoryGoal Configuration
        modelBuilder.Entity<CategoryGoal>(entity =>
        {
            entity.ToTable("category_goals");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // One-to-Many: Category -> CategoryGoals
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Goals)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction Configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // TransactionSplit Configuration (ONE-TO-MANY)
        modelBuilder.Entity<TransactionSplit>(entity =>
        {
            entity.ToTable("transaction_splits");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // One-to-Many: Transaction -> TransactionSplits
            entity.HasOne(e => e.Transaction)
                .WithMany(t => t.Splits)
                .HasForeignKey(e => e.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Category -> TransactionSplits
            entity.HasOne(e => e.Category)
                .WithMany(c => c.TransactionSplits)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TransactionCategory Configuration (ONE-TO-ONE)
        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.ToTable("transaction_categories");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // One-to-One: Transaction -> TransactionCategory
            entity.HasOne(e => e.Transaction)
                .WithOne(t => t.TransactionCategory)
                .HasForeignKey<TransactionCategory>(e => e.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Category -> TransactionCategories
            entity.HasOne(e => e.Category)
                .WithMany(c => c.TransactionCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint on transaction_id
            entity.HasIndex(e => e.TransactionId).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}