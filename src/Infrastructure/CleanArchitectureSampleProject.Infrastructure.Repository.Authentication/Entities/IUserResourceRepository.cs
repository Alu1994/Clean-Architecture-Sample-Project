using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public interface IUserResourceRepository
{
    Task<Results<FrozenSet<UserResource>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<UserResource, BaseError>> GetById(int id, CancellationToken cancellation = default);


    Task<Results<IReadOnlyCollection<UserResource>, BaseError>> GetUsersResourcesBy(int userId, CancellationToken cancellation = default);
    Task<Results<IReadOnlyCollection<UserResourceView>, BaseError>> GetCompleteUsersResourcesBy(Expression<Func<UserResource, bool>> filter, CancellationToken cancellation = default);


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

    public async Task<Results<IReadOnlyCollection<UserResourceView>, BaseError>> GetCompleteUsersResourcesBy(Expression<Func<UserResource, bool>> filter, CancellationToken cancellation = default)
    {
        try
        {
            var usersResources = await _context.UsersResources.AsNoTracking()
                .Include(x => x.User).AsNoTracking()
                .Include(x => x.Resource).AsNoTracking()
                .Where(filter).AsNoTracking()
                .Select(x => new UserResourceView
                {
                    UserResourceId = x.Id,
                    UserId = x.UserId,
                    UserName = x.User.Name,
                    ResourceId = x.ResourceId,
                    ResourceName = x.Resource.Name,
                    CanRead = x.CanRead,
                    CanWrite = x.CanWrite,
                    CanDelete = x.CanDelete,
                })
                .ToListAsync(cancellation);
            return usersResources.AsReadOnly();
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting Complete UserResource ': {ex.Message}");
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

public record UserResourceView
{
    public int UserResourceId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ResourceId { get; set; }
    public string ResourceName { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}