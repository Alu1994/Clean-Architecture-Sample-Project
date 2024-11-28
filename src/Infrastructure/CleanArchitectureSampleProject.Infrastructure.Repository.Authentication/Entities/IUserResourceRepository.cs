using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public interface IUserResourceRepository
{
    Task<Results<FrozenSet<UserResource>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<UserResource, BaseError>> GetById(int id, CancellationToken cancellation = default);

    Task<Results<IReadOnlyCollection<UserResource>, BaseError>> GetUsersResourcesBy(int userId, CancellationToken cancellation = default);

    Task<ValidationResult> Insert(UserResource userResource, CancellationToken cancellation);
    Task<ValidationResult> Update(UserResource userResource, CancellationToken cancellation);
}

public sealed class UserResourceRepository : IUserResourceRepository
{
    private readonly AuthenticationDataContext _context;

    public UserResourceRepository(AuthenticationDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Results<FrozenSet<UserResource>, BaseError>> Get(CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Results<UserResource, BaseError>> GetById(int id, CancellationToken cancellation = default)
    {
        try
        {
            var usersResource = await _context.UsersResources.Where(x => x.Id == id).FirstAsync();
            return usersResource;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting UserResource '{id}': {ex.Message}");
        }
    }

    public async Task<Results<IReadOnlyCollection<UserResource>, BaseError>> GetUsersResourcesBy(int userId, CancellationToken cancellation = default)
    {
        try
        {
            var usersResources = await _context.UsersResources.Where(x => x.UserId == userId).ToListAsync();
            return usersResources.AsReadOnly();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting UserResource with UserId '{userId}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Insert(UserResource userResource, CancellationToken cancellation)
    {
        try
        {
            await _context.UsersResources.AddAsync(userResource, cancellation);
            await _context.SaveChangesAsync(true, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting UserResource with User '{userResource.UserId}': {ex.Message}");
        }
    }

    public Task<ValidationResult> Update(UserResource userResource, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}