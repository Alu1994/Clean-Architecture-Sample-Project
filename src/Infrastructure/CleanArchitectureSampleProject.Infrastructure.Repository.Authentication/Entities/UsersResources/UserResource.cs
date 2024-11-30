using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

public sealed class UserResource
{
    public int Id { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
    public DateTime CreationDate { get; set; }


    public int UserId { get; set; }
    public User User { get; set; }
    public int ResourceId { get; set; }
    public Resource Resource { get; set; }
}
