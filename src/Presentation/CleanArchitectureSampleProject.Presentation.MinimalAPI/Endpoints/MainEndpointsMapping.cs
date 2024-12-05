using CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Middlewares;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;

public static class MainEndpointsMapping
{
    public static string DefaultContentType => "application/json";
    public static string BadRequestDescription => HttpStatusCode.BadRequest.ToString();

    public static short Success => StatusCodes.Status200OK;
    public static short Created => StatusCodes.Status201Created;
    public static short BadRequest => StatusCodes.Status400BadRequest;
    public static short Unauthorized => StatusCodes.Status401Unauthorized;
    public static short Forbidden => StatusCodes.Status403Forbidden;

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapSells();
        app.MapProducts();
        app.MapCategories();

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
            .Produces<UnauthorizedResponse>(Unauthorized, DefaultContentType)
            .Produces<ForbiddenResponse>(Forbidden, DefaultContentType)
            .AddFluentValidationAutoValidation()
            .WithOpenApi();
    }
}
