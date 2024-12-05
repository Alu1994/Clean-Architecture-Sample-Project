using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public partial class AuthenticationDataContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Resource> Resources { get; set; } = null!;
    public DbSet<UserResource> UsersResources { get; set; } = null!;
}