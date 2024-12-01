using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;

public sealed class SellConfiguration : IEntityTypeConfiguration<Sell>
{
    public void Configure(EntityTypeBuilder<Sell> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.TotalValue).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.CreationDate).HasDefaultValue(DateTime.UtcNow);

        // One-to-Many Relationship
        builder.HasMany(u => u.Items)
              .WithOne(o => o.Sell)
              .HasForeignKey(o => o.SellId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
    }
}
