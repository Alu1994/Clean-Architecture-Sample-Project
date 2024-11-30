using CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapGetById(this WebApplication app)
    {
        app.MapGet($"/{Controller}/{{categoryId:Guid}}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation) =>
        {
            return await GetById(logger, categoryUseCases, categoryId, cancellation);
        })
        .Produces<CategoryOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Get {Controller} By Id", TagName)
        .RequireAuthorization(AuthenticationSetup.CategoryCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting category by id.";

        var result = await categoryUseCases.GetCategoryById(categoryId, cancellation);
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
