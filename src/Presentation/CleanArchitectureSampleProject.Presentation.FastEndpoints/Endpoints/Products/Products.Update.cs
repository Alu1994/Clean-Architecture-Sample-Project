using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Products;

public sealed class UpdateProducts(ILogger<UpdateProducts> logger, IProductUseCases productUseCases) :
    Endpoint<UpdateProductInput, Microsoft.AspNetCore.Http.HttpResults.Results<Ok<UpdateProductOutput>, ProblemHttpResult>>
{
    private readonly ILogger<UpdateProducts> _logger = logger;
    private readonly IProductUseCases _productUseCases = productUseCases;

    public override void Configure()
    {
        Put("/products");
        Description(b => b
            .Accepts<UpdateProductInput>(false, "application/json")
            .Produces<UpdateProductOutput>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(ProductCanWritePolicy));
        Validator<UpdateProductInputValidator>();
    }

    public override async Task<Microsoft.AspNetCore.Http.HttpResults.Results<Ok<UpdateProductOutput>, ProblemHttpResult>> ExecuteAsync(UpdateProductInput product, CancellationToken cancellation)
    {
        var result = await _productUseCases.UpdateProduct(product, cancellation);

        if (result.IsSuccess)
            return TypedResults.Ok(result.Success!);

        _logger.LogErrorList(result.Error!);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: "Error while updating Product.",
            detail: result.Error!.ToDetailMessage(),
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}