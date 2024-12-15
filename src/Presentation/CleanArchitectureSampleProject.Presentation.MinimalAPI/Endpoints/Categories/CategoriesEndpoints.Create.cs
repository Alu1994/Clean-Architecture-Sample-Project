using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static RouteGroupBuilder MapCreate(this RouteGroupBuilder app)
    {
        app.MapPost("", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation) =>
        {
            return await Create(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<CreateCategoryInput>(DefaultContentType)
        .Produces<CategoryOutput>(Created, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Create Category", TagName)
        .RequireAuthorization(CategoryCanWritePolicy);
        
        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation)
    {
        var result = await categoryUseCases.CreateCategory(category, cancellation);
        return result.ToCreatedOrErrorResult(logger, "Error while creating new category.");
    }
}