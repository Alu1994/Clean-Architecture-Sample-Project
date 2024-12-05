using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Logins;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Resources;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;

public static class MainEndpointsMapping
{
    public const string ContentType = "application/json";
    public const short Success = StatusCodes.Status200OK;
    public const short Created = StatusCodes.Status201Created;
    public const short BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapLogin();
        app.MapUsers();
        app.MapResources();
        app.MapUsersResources();

        return app;
    }

    public static RouteHandlerBuilder WithConfigSummaryInfo(this RouteHandlerBuilder builder, string description, params string[] tagName)
    {
        return builder
            .WithName(description)
            .WithDescription(description)
            .WithSummary(description)
            .WithDisplayName(description)
            .WithTags(tagName)
            .AddFluentValidationAutoValidation()
            .WithOpenApi();
    }
}
