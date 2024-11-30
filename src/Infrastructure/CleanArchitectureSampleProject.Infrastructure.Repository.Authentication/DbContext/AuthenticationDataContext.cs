using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        modelBuilder.ApplyConfiguration(new ResourceConfiguration());
        modelBuilder.ApplyConfiguration(new UserResourceConfiguration());
    }
}

public static class Errors
{
    public enum DatabaseErrorCodes : short
    {
        DuplicatedKey = 23505
    }

    public static bool IsDuplicatedKeyException(this Exception ex)
    {
        return ex.InnerException is PostgresException postgresEx && 
            postgresEx.SqlState == ((short)DatabaseErrorCodes.DuplicatedKey).ToString();
    }
}