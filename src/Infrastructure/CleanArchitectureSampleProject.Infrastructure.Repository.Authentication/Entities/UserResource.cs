namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public sealed class UserResource
{
    public int Id { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    public int ResourceId { get; set; }
    public Resource Resource { get; set; }
}
