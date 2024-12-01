using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.Quantity);
        builder.Property(e => e.Value).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CreationDate).HasDefaultValue(DateTime.UtcNow);

        // One-to-Many Relationship
        builder.HasMany(u => u.Items)
              .WithOne(o => o.Product)
              .HasForeignKey(o => o.ProductId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
    }
}
