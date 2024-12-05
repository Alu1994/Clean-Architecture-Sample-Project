using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static RouteGroupBuilder MapGetAll(this RouteGroupBuilder app)
    {
        app.MapGet("", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation) =>
        {
            return await GetAll(logger, categoryUseCases, cancellation);
        })
        .Produces<FrozenSet<CategoryOutput>>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get All Categories", TagName)
        .RequireAuthorization(CategoryCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CancellationToken cancellation)
    {
        var result = await categoryUseCases.GetCategories(cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting all categories.");
    }
}