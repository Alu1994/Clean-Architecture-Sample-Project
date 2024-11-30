using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;

public sealed class Resource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }

    public ICollection<UserResource> UserResources { get; set; } = new List<UserResource>();
}
