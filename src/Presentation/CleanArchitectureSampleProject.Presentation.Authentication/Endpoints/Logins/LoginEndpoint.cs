using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Setups;
using System.Net;
using static CleanArchitectureSampleProject.CrossCuttingConcerns.PolicyExtensions;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Logins;

public static class LoginEndpoint
{
    public readonly record struct Logging;
    private const string TagName = "Logins";
    private const string Controller = "login";
    private const byte StartItems = 3;
    private const byte AccessCount = 3;

    public static WebApplication MapLogin(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (
            IUserResourceRepository userResourceRepository,
            LoginRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await Login(userResourceRepository, request, cancellationToken);
            return result;
        })
        .Produces<string>(Success, ContentType)
        .WithConfigSummaryInfo("Generate Login Token", TagName);

        return app;
    }

    private static async Task<IResult> Login(IUserResourceRepository userResourceRepository, LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await userResourceRepository.GetCompleteUsersResourcesBy(x => 
            x.User.Email == request.Email &&
            x.User.Password == request.Password, 
            cancellationToken);
        if (result.IsFail || result.Success is { Count: 0 })
        {
            return Results.Problem(detail: $"User '{request.Email}' not found.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Error while generating token.",
                type: HttpStatusCode.BadRequest.ToString());
        }

        var resourcesCount = result.Success!.Count;
        var capacity = StartItems + (resourcesCount * AccessCount);
        List<Claim> claims = new(capacity)
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
        };
        foreach (var userResource in result.Success!)
        {
            claims.AddRange(userResource.ToClaims());
        }

        var (tokenValue, expiresIn, securityToken) = claims.ToSecurityTokenDescriptor();
        return Results.Ok(new TokenResultResponse(
            tokenValue,
            securityToken.ValidFrom,
            securityToken.ValidTo,
            securityToken.Issuer,
            expiresIn
        ));
    }
}
