using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

public sealed class UserResourceConfiguration : IEntityTypeConfiguration<UserResource>
{
    public void Configure(EntityTypeBuilder<UserResource> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CanRead);
        builder.Property(e => e.CanWrite);
        builder.Property(e => e.CanDelete);
        builder.Property(e => e.CreationDate);
        builder.HasIndex(e => new { e.UserId, e.ResourceId }).IsUnique();
    }
}
