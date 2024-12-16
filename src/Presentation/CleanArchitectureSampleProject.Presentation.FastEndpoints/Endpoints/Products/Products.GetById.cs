using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Products;

public sealed class GetByIdProducts(ILogger<GetByIdProducts> logger, IProductUseCases productUseCases) :
    EndpointWithoutRequest<Results<Ok<GetProductOutput>, NoContent, ProblemHttpResult>>
{
    private readonly ILogger<GetByIdProducts> _logger = logger;
    private readonly IProductUseCases _productUseCases = productUseCases;

    public override void Configure()
    {
        Get("/products/{productId:Guid}");
        Description(b => b
            .Produces<GetProductOutput>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(ProductCanReadPolicy));
    }

    public override async Task<Results<Ok<GetProductOutput>, NoContent, ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var productId = Route<Guid>("productId");
        var result = await _productUseCases.GetProductById(productId, cancellation);

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