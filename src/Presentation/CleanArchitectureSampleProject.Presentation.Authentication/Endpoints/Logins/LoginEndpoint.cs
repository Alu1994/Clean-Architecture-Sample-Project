using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Logins;

public static class LoginEndpoint
{
    public readonly record struct Logging;
    private const string TagName = "Logins";
    private const string Controller = "login";

    public static WebApplication MapLogin(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (
            IUserRepository userRepository,
            IResourceRepository resourceRepository,
            IUserResourceRepository userResourceRepository,
            LoginRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await GenerateToken(userRepository, resourceRepository, userResourceRepository, request, cancellationToken);
            if (result.IsSuccess)
            {
                return Results.Ok(new { access_token = result.Success });
            }
            return Results.Problem(
                detail: $"User '{request.Email}' not found.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Error while generating token.",
                type: HttpStatusCode.BadRequest.ToString());
        })
        .Produces<string>(Success, ContentType)
        .WithConfigSummaryInfo("Generate Login Token", TagName);

        return app;
    }

    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    private static async Task<Results<string, BaseError>> GenerateToken(IUserRepository userRepository, IResourceRepository resourceRepository, IUserResourceRepository userResourceRepository, LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await userRepository.GetUserByEmailAndPassword(request.Email, request.Password, cancellationToken);
        if (result.IsFail) return result.Error!;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
        };

        var resultUsersResources = await userResourceRepository.GetUsersResourcesBy(result.Success.Id, cancellationToken);
        if (resultUsersResources.IsFail) return resultUsersResources.Error!;
        foreach (var usersResource in resultUsersResources.Success)
        {
            var resultResource = await resourceRepository.GetById(usersResource.ResourceId, cancellationToken);
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
}
