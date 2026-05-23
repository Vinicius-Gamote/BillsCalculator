using GastosControl.Application.Abstractions;
using GastosControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GastosControl.Infrastructure.Persistence;

public sealed class GastosDbContext : DbContext, IUnitOfWork
{
    public GastosDbContext(DbContextOptions<GastosDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<FinancialTransaction> Transactions => Set<FinancialTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Name).HasMaxLength(120).IsRequired();
            builder.Property(user => user.Email).HasMaxLength(180).IsRequired();
            builder.Property(user => user.PasswordHash).HasMaxLength(256).IsRequired();
            builder.Property(user => user.CreatedAt).IsRequired();

            builder.HasIndex(user => user.Email).IsUnique();

            builder.HasMany(user => user.Categories)
                .WithOne(category => category.User)
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(user => user.Transactions)
                .WithOne(transaction => transaction.User)
                .HasForeignKey(transaction => transaction.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(user => user.Categories).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(user => user.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.ToTable("Categories");
            builder.HasKey(category => category.Id);

            builder.Property(category => category.Name).HasMaxLength(80).IsRequired();
            builder.Property(category => category.Color).HasMaxLength(16).IsRequired();
            builder.Property(category => category.Type).HasConversion<int>().IsRequired();
            builder.Property(category => category.IsArchived).HasDefaultValue(false);
            builder.Property(category => category.CreatedAt).IsRequired();

            builder.HasIndex(category => new { category.UserId, category.Name }).IsUnique();

            builder.HasMany(category => category.Transactions)
                .WithOne(transaction => transaction.Category)
                .HasForeignKey(transaction => transaction.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(category => category.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<FinancialTransaction>(builder =>
        {
            builder.ToTable("Transactions");
            builder.HasKey(transaction => transaction.Id);

            builder.Property(transaction => transaction.Type).HasConversion<int>().IsRequired();
            builder.Property(transaction => transaction.Description).HasMaxLength(160).IsRequired();
            builder.Property(transaction => transaction.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(transaction => transaction.OccurredOn).HasColumnType("date").IsRequired();
            builder.Property(transaction => transaction.Notes).HasMaxLength(500);
            builder.Property(transaction => transaction.CreatedAt).IsRequired();

            builder.HasIndex(transaction => new { transaction.UserId, transaction.OccurredOn });
            builder.HasIndex(transaction => new { transaction.UserId, transaction.Type });
            builder.HasIndex(transaction => new { transaction.UserId, transaction.CategoryId });
        });
    }
}
