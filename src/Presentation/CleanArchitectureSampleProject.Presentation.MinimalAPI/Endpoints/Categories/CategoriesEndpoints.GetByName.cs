namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static RouteGroupBuilder MapGetByName(this RouteGroupBuilder app)
    {
        app.MapGet("/by-name/{categoryName}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, categoryUseCases, categoryName, cancellation);
        })
        .Produces<CategoryOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get Category By Name", TagName)
        .RequireAuthorization(CategoryCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetByName(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation)
    {
        var result = await categoryUseCases.GetCategoryByName(categoryName, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting category by name.");
    }
}
