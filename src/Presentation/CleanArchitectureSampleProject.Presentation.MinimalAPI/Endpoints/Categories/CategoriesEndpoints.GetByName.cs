namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/by-name/{{categoryName}}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, categoryUseCases, categoryName, cancellation);
        })
        .Produces<CategoryOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .Produces<UnauthorizedResponse>(Unauthorized, ContentType)
        .WithConfigSummaryInfo($"Get {Controller} By Name", TagName)
        .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetByName(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting category by name.";

        var result = await categoryUseCases.GetCategoryByName(categoryName, cancellation);
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
