using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Logins;

public static class LoginEndpoint
{
    public readonly record struct Logging;
    private const string TagName = "Logins";
    private const string Controller = "login";

    private const byte StartItems = 3;
    private const byte AccessCount = 3;
    private const byte defaultExpiresIn = 60;

    public static WebApplication MapLogin(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (
            IUserResourceRepository userResourceRepository,
            LoginRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await GenerateToken(userResourceRepository, request, cancellationToken);
            return result;
        })
        .Produces<string>(Success, ContentType)
        .WithConfigSummaryInfo("Generate Login Token", TagName);

        return app;
    }

    private static async Task<IResult> GenerateToken(IUserResourceRepository userResourceRepository, LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await userResourceRepository.GetCompleteUsersResourcesBy(x => x.User.Email == request.Email && x.User.Password == request.Password, cancellationToken);
        if (result.IsFail)
        {
            return Results.Problem(
                detail: $"User '{request.Email}' not found.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Error while generating token.",
                type: HttpStatusCode.BadRequest.ToString()
            );
        }
        var resourcesCount = result.Success!.Count;
        List<Claim> claims = new(StartItems + (resourcesCount * AccessCount))
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
        };
        foreach (var userResource in result.Success!)
            claims.AddRange(userResource.ToClaims());

        var tokenDescriptor = GenerateSecurityTokenDescriptor(claims, defaultExpiresIn);
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenValue = tokenHandler.WriteToken(securityToken);

        return Results.Ok(new TokenResult(
            tokenValue,
            securityToken.ValidFrom,
            securityToken.ValidTo,
            securityToken.Issuer,
            defaultExpiresIn
        ));
    }

    private static readonly byte[] SymmetricKey = "MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray();

    private static SecurityTokenDescriptor GenerateSecurityTokenDescriptor(List<Claim> claims, byte expiresIn)
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiresIn),
            Issuer = "https://id.cleanarchsampleproject.com.br",
            Audience = "https://cleanarchsampleproject.com.br",
            NotBefore = DateTime.UtcNow.Add(TimeSpan.FromSeconds(5)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SymmetricKey), SecurityAlgorithms.HmacSha256Signature),
        };
    }
}

internal record TokenResult(string AccessToken, DateTime ValidFrom, DateTime ValidTo, string Issuer, byte ExpiresInMinutes);