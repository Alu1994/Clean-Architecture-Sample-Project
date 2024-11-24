namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CategoryInput category, CancellationToken cancellation) =>
        {
            return await Create(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<CategoryInput>(ContentType)
        .Produces<CategoryOutput>(Created, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Create {Controller}", TagName);
        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CategoryInput category, CancellationToken cancellation)
    {
        const string errorTitle = "Error while creating new category.";

        var result = await categoryUseCases.CreateCategory(category, cancellation);
        return result.Match(success => Results.Created($"/{Controller}", success),
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
