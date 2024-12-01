using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;

public sealed class SellItemConfiguration : IEntityTypeConfiguration<SellItem>
{
    public void Configure(EntityTypeBuilder<SellItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.Value).HasColumnType("decimal(18,2)").IsRequired();
    }
}
