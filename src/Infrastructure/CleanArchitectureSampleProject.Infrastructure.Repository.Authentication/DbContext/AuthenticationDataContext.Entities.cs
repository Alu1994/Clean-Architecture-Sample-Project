using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public partial class AuthenticationDataContext
{
    public DbSet<User> Users { get; set; } = null!;
}