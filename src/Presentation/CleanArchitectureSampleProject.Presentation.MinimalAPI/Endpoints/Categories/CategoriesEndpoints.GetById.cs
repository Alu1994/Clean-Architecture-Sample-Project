using CleanArchitectureSampleProject.Core.Application.Outputs.Products;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static RouteGroupBuilder MapGetById(this RouteGroupBuilder app)
    {
        app.MapGet("/{categoryId:Guid}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation) =>
        {
            return await GetById(logger, categoryUseCases, categoryId, cancellation);
        })
        .Produces<CategoryOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get Category By Id", TagName)
        .RequireAuthorization(CategoryCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation)
    {
        var result = await categoryUseCases.GetCategoryById(categoryId, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting category by id.");
    }
}
