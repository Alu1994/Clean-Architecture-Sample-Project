using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class ProductConfiguration;
//    : IEntityTypeConfiguration<Product>
//{
//    public void Configure(EntityTypeBuilder<Product> builder)
//    {
//        // Table Configuration
//        builder.ToTable("Products");
//        builder.HasKey(x => x.Id);

//        // Column Configuration
//        builder.Property(x => x.Name);
//        builder.Property(x => x.Description);
//        builder.Property(x => x.Quantity);
//        builder.Property(x => x.Value);
//        builder.Property(x => x.CreationDate);

//        builder.HasOne(x => x.Category)
//               .WithMany(x => x.Products)
//               .HasForeignKey(x => x.Id);

//        // TODO: Descomentar quando o tipo ORDER_STATUS for removido do banco
//        // builder
//        //     .Property(x => x.Status)
//        //     .HasConversion(
//        //         x => OrderStatusMapper.PgsqlMapper[x],
//        //         x => OrderStatusMapper.PgsqlMapper.FirstOrDefault(y => y.Value == x).Key);
//    }
//}
