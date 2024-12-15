using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;

public sealed class GetByIdCategories(ILogger<GetByIdCategories> logger, ICategoryUseCases categoryUseCases) :
    EndpointWithoutRequest<Results<Ok<CategoryOutput>, NoContent, ProblemHttpResult>>
{
    private readonly ILogger<GetByIdCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Get("/categories/{categoryId:Guid}");
        Description(b => b
            .Produces<CategoryOutput>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(CategoryCanReadPolicy));
    }

    public override async Task<Results<Ok<CategoryOutput>, NoContent, ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var categoryId = Route<Guid>("categoryId");
        var result = await _categoryUseCases.GetCategoryById(categoryId, cancellation);

        if (result.IsSuccess && result.Success is null)
            return TypedResults.NoContent();

        if (result.IsSuccess)
            return TypedResults.Ok(result.Success!);

        _logger.LogError(message: result.Error!.Message);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: result.Error!.Message,
            detail: result.Error!.Message,
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}