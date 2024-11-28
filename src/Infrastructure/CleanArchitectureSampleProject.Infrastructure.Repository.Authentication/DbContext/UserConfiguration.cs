﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Password).IsRequired();
        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.CreationDate);

        // One-to-Many Relationship
        builder.HasMany(u => u.UserResources)
              .WithOne(o => o.User)
              .HasForeignKey(o => o.UserId)
              .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
    }
}