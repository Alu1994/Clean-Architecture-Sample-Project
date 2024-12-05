using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static RouteGroupBuilder MapUpdate(this RouteGroupBuilder app)
    {
        app.MapPut("", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, UpdateCategoryInput category, CancellationToken cancellation) =>
        {
            return await Update(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<UpdateCategoryInput>(DefaultContentType)
        .Produces<CategoryOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Update Category", TagName)
        .RequireAuthorization(CategoryCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, UpdateCategoryInput category, CancellationToken cancellation)
    {
        var result = await categoryUseCases.UpdateCategory(category, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while updating category.");
    }
}

