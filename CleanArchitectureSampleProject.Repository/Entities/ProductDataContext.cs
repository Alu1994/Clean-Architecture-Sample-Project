using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Repository.Entities;

public class ProductDataContext : DbContext
{
    public ProductDataContext(DbContextOptions<ProductDataContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category table
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreationDate);

            // One-to-Many Relationship
            entity.HasMany(u => u.Products)
                  .WithOne(o => o.Category)
                  .HasForeignKey(o => o.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
        });

        // Configure Product table
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreationDate);
        });
    }


    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
}