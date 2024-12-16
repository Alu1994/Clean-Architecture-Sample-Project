using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Http = Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;

public sealed class GetAllCategories(ILogger<GetAllCategories> logger, ICategoryUseCases categoryUseCases) : 
    EndpointWithoutRequest<Http.Results<Http.Ok<FrozenSet<CategoryOutput>>, Http.NoContent, Http.ProblemHttpResult>>
{
    private readonly ILogger<GetAllCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Get("/categories");
        Description(b => b
            .Produces<FrozenSet<CategoryOutput>>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(CategoryCanReadPolicy));
    }

    public override async Task<Http.Results<Http.Ok<FrozenSet<CategoryOutput>>, Http.NoContent, Http.ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var result = await _categoryUseCases.GetCategories(cancellation);

        if (result.IsSuccess && result.Success!.Count is 0)
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