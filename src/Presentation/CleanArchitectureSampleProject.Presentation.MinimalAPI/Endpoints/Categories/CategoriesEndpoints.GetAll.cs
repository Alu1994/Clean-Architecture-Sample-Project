using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapGetAll(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation) =>
        {
            return await GetAll(logger, categoryUseCases, cancellation);
        })
        .Produces<FrozenSet<CategoryOutput>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get All Categories", TagName);
        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting all categories.";

        var result = await categoryUseCases.GetCategories(cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogBaseError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }
}
