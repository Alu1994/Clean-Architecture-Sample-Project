using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;

public sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.CreationDate);

        // One-to-Many Relationship
        builder.HasMany(u => u.UserResources)
              .WithOne(o => o.Resource)
              .HasForeignKey(o => o.ResourceId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
    }
}
