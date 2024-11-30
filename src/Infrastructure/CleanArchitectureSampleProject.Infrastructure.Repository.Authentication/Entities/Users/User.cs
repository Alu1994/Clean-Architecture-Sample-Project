using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreationDate { get; set; }

    public ICollection<UserResource> UserResources { get; set; } = new List<UserResource>();
}
