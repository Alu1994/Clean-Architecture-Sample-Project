using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;

public interface IUserRepository
{
    Task<Results<FrozenSet<User>, BaseError>> Get(CancellationToken cancellation = default);
    Task<Results<User, BaseError>> GetById(int id, CancellationToken cancellation = default);
    Task<Results<User, BaseError>> GetUserByName(string name, CancellationToken cancellation = default);
    Task<Results<User, BaseError>> GetUserByEmailAndPassword(string email, string password, CancellationToken cancellation = default);
    Task<Results<BaseError>> Insert(User user, CancellationToken cancellation);
    Task<ValidationResult> Update(User user, CancellationToken cancellation);
}

public sealed class UserRepository : IUserRepository
{
    private readonly AuthenticationDataContext _context;

    public UserRepository(AuthenticationDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Results<FrozenSet<User>, BaseError>> Get(CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task<Results<User, BaseError>> GetById(int id, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Results<User, BaseError>> GetUserByName(string name, CancellationToken cancellation = default)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.Name == name, cancellation);
            return user;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting User '{name}': {ex.Message}");
        }
    }

    public async Task<Results<User, BaseError>> GetUserByEmailAndPassword(string email, string password, CancellationToken cancellation = default)
    {
        try
        {
            var user = await _context.Users.FirstAsync(x => x.Email == email && x.Password == password, cancellation);
            return user;
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Getting User '{email}': {ex.Message}");
        }
    }

    public async Task<Results<BaseError>> Insert(User user, CancellationToken cancellation)
    {
        try
        {
            await _context.Users.AddAsync(user, cancellation);
            await _context.SaveChangesAsync(true, cancellation);
            return ResultStates.Ok;
        }
        catch (DbUpdateException ex) when (ex.IsDuplicatedKeyException())
        {
            return new BaseError($"User '{user.Name}' already exists.: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new BaseError($"Error while Inserting User '{user.Name}': {ex.Message}");
        }
    }

    public Task<ValidationResult> Update(User user, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}