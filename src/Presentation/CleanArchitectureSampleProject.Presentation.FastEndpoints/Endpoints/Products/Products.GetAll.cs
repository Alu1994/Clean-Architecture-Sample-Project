using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Http = Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Products;

public sealed class GetAllProducts(ILogger<GetAllProducts> logger, IProductUseCases productUseCases) : 
    EndpointWithoutRequest<Http.Results<Http.Ok<FrozenSet<GetProductOutput>>, Http.NoContent, Http.ProblemHttpResult>>
{
    private readonly ILogger<GetAllProducts> _logger = logger;
    private readonly IProductUseCases _productUseCases = productUseCases;

    public override void Configure()
    {
        Get("/products");
        Description(b => b
            .Produces<FrozenSet<GetProductOutput>>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(ProductCanReadPolicy));
    }

    public override async Task<Http.Results<Http.Ok<FrozenSet<GetProductOutput>>, Http.NoContent, Http.ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var result = await _productUseCases.GetProducts(cancellation);

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