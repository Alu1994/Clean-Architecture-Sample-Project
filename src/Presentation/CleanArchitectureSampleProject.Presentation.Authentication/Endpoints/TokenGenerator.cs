using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;

public sealed class TokenGenerator(IUserRepository userRepository, IResourceRepository resourceRepository, IUserResourceRepository userResourceRepository)
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IResourceRepository _resourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
    private readonly IUserResourceRepository _userResourceRepository = userResourceRepository ?? throw new ArgumentNullException(nameof(userResourceRepository));

    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    public async Task<Results<string, BaseError>> GenerateToken(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserByEmailAndPassword(request.Email, request.Password, cancellationToken);
        if (result.IsFail) return result.Error!;
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),            
        };

        var resultUsersResources = await _userResourceRepository.GetUsersResourcesBy(result.Success.Id, cancellationToken);
        if (resultUsersResources.IsFail) return resultUsersResources.Error!;
        foreach (var usersResource in resultUsersResources.Success)
        {
            var resultResource = await _resourceRepository.GetById(usersResource.ResourceId, cancellationToken);
            if (resultResource.IsFail) return resultResource.Error!;

            var resource = resultResource.Success;

            claims.Add(new($"{resource.Name.ToLowerInvariant()}{nameof(usersResource.CanRead).ToLowerInvariant()}claim", usersResource.CanRead.ToString()));
            claims.Add(new($"{resource.Name.ToLowerInvariant()}{nameof(usersResource.CanWrite).ToLowerInvariant()}claim", usersResource.CanWrite.ToString()));
            claims.Add(new($"{resource.Name.ToLowerInvariant()}{nameof(usersResource.CanDelete).ToLowerInvariant()}claim", usersResource.CanDelete.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://id.cleanarchsampleproject.com.br",
            Audience = "https://cleanarchsampleproject.com.br",
            NotBefore = DateTime.UtcNow.Add(TimeSpan.FromSeconds(5)),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(SymmetricKey),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<CreateUserResponse> CreateUser(CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = userRequest.Name,
            Email = userRequest.Email,
            Password = userRequest.Password,
            CreationDate = userRequest.CreationDate
        };
        var isSuccess = await _userRepository.Insert(user, cancellationToken);
        return new CreateUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = user.Password,
            CreationDate = user.CreationDate
        };
    }

    public async Task<CreateResourceResponse> CreateResource(CreateResourceRequest resourceRequest, CancellationToken cancellationToken)
    {
        var resource = new Resource
        {
            Name = resourceRequest.Name
        };
        var isSuccess = await _resourceRepository.Insert(resource, cancellationToken);
        return new CreateResourceResponse
        {
            Id = resource.Id,
            Name = resource.Name
        };
    }

    public async Task<CreateUserResourceResponse> CreateUserResource(CreateUserResourceRequest userResourceRequest, CancellationToken cancellationToken)
    {
        var userResource = new UserResource
        {
            UserId = userResourceRequest.UserId,
            ResourceId = userResourceRequest.ResourceId,
            CanRead = userResourceRequest.CanRead,
            CanWrite = userResourceRequest.CanWrite,
            CanDelete = userResourceRequest.CanDelete,
        };
        var isSuccess = await _userResourceRepository.Insert(userResource, cancellationToken);
        return new CreateUserResourceResponse
        {
            Id = userResource.Id,
            UserId = userResource.UserId,
            ResourceId = userResource.ResourceId,
            CanRead = userResource.CanRead,
            CanWrite = userResource.CanWrite,
            CanDelete = userResource.CanDelete,
        };
    }
}


public record CreateUserRequest
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreationDate { get; set; }
}
public record CreateResourceRequest
{
    public string Name { get; set; }
}
public record CreateUserResourceRequest
{
    public int UserId { get; set; }
    public int ResourceId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}



public record CreateUserResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreationDate { get; set; }
}
public record CreateResourceResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public record CreateUserResourceResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ResourceId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}
