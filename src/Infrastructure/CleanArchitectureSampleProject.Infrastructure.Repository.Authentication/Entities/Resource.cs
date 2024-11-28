namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public sealed class Resource
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<UserResource> UserResources { get; set; } = new List<UserResource>();
}
