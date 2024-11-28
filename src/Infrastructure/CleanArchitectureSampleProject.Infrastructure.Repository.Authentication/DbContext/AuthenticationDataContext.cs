using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public partial class AuthenticationDataContext : DbContext
{
    public AuthenticationDataContext(DbContextOptions<AuthenticationDataContext> options) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
