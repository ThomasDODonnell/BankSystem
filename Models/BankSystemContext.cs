using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API.Models;
public class BankSystemContext: DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryGoal> CategoryGoals => Set<CategoryGoal>();
    public DbSet<TransactionSplit> TranactionSplits => Set<TransactionSplit>();
    public DbSet<TransactionCategory> TranactionCategories => Set<TransactionsCategory>();
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
                  .WithMany(ts => ts.TransactionSplit)
                  .HasForeignKey<TransactionSplit>(cat => cat.CategoryId);
            entity.HasOne(trans => trans.Transaction) 
                  .WithMany(ts => ts.TransactionSplit)
                  .HasForeignKey<TransactionSplit>(goal => goal.TransactionId);
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.ToTable("transaction_categories");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

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
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(7);
            entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // CategoryGoal Configuration
        modelBuilder.Entity<CategoryGoal>(entity =>
        {
            entity.ToTable("category_goals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Period).HasColumnName("period").HasMaxLength(20).IsRequired();
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsSplit).HasColumnName("is_split").HasDefaultValue(false);
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // TransactionSplit Configuration (ONE-TO-MANY)
        modelBuilder.Entity<TransactionSplit>(entity =>
        {
            entity.ToTable("transaction_splits");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notes).HasColumnName("notes");
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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
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