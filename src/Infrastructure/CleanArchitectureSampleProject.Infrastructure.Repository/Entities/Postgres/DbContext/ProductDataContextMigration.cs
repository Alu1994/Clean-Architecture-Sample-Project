using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities;

public partial class ProductDataContextMigration : DbContext
{
    public ProductDataContextMigration(DbContextOptions<ProductDataContextMigration> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());

        modelBuilder.ApplyConfiguration(new SellConfiguration());
        modelBuilder.ApplyConfiguration(new SellItemConfiguration());
    }
}
