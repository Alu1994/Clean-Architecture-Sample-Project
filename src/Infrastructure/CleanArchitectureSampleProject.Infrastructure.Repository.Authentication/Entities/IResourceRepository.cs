using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

public interface IResourceRepository
{
    Task<Results<FrozenSet<Resource>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<Resource, BaseError>> GetById(int id, CancellationToken cancellation = default);
    Task<Results<Resource, BaseError>> GetResourceByName(string name, CancellationToken cancellation = default);
    Task<ValidationResult> Insert(Resource resource, CancellationToken cancellation);
    Task<ValidationResult> Update(Resource resource, CancellationToken cancellation);
}

public sealed class ResourceRepository : IResourceRepository
{
    private readonly AuthenticationDataContext _context;

    public ResourceRepository(AuthenticationDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Results<FrozenSet<Resource>, BaseError>> Get(CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Results<Resource, BaseError>> GetById(int id, CancellationToken cancellation = default)
    {
        try
        {
            var resource = await _context.Resources.Where(x => x.Id == id).FirstAsync();
            return resource;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting Resource '{id}': {ex.Message}");
        }
    }

    public async Task<Results<Resource, BaseError>> GetResourceByName(string name, CancellationToken cancellation = default)
    {
        try
        {
            var resource = await _context.Resources.FirstAsync(x => x.Name == name, cancellation);
            return resource;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting Resource '{name}': {ex.Message}");
        }
    }

    public async Task<ValidationResult> Insert(Resource resource, CancellationToken cancellation)
    {
        try
        {
            await _context.Resources.AddAsync(resource, cancellation);
            await _context.SaveChangesAsync(true, cancellation);
            return ValidationResult.Success!;
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error while Inserting Resource '{resource.Name}': {ex.Message}");
        }
    }

    public Task<ValidationResult> Update(Resource resource, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}