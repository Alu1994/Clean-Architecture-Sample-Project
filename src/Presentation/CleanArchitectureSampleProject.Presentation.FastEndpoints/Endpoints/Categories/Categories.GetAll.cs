using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Middlewares;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;


public class GetAllCategories(ILogger<CreateCategories> logger, ICategoryUseCases categoryUseCases) : 
    EndpointWithoutRequest<Results<Ok<FrozenSet<CategoryOutput>>, NoContent, ProblemHttpResult>>
{
    private readonly ILogger<CreateCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Get("/categories");
        Description(b => b
            .Produces<CategoryOutput>(200, "application/json")
            .Produces(204)
            .ProducesProblemDetails(400, "application/json")
            .Produces<UnauthorizedResponse>(401, "application/json")
            .Produces<ForbiddenResponse>(403, "application/json"));

        var claim = GetAuthClaim(CategoryCanReadPolicy);
        Policy(x => x.RequireClaim(claim.Name, claim.AcceptedValues));
    }

    public override async Task<Results<Ok<FrozenSet<CategoryOutput>>, NoContent, ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
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