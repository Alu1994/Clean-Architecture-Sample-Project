using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Products;

public sealed class CreateProducts(ILogger<CreateProducts> logger, IProductUseCases productUseCases) : 
    Endpoint<CreateProductInput, Microsoft.AspNetCore.Http.HttpResults.Results<Created<CreateProductOutput>, ProblemHttpResult>>
{
    private readonly ILogger<CreateProducts> _logger = logger;
    private readonly IProductUseCases _productUseCases = productUseCases;

    public override void Configure()
    {
        Post("/products");
        Description(b => b
            .Accepts<CreateProductInput>(false, "application/json")
            .Produces<CreateProductOutput>(StatusCodes.Status201Created, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(ProductCanWritePolicy));
        Validator<CreateProductInputValidator>();
    }

    public override async Task<Microsoft.AspNetCore.Http.HttpResults.Results<Created<CreateProductOutput>, ProblemHttpResult>> ExecuteAsync(CreateProductInput product, CancellationToken cancellation)
    {
        var result = await _productUseCases.CreateProduct(product, cancellation);

        if (result.IsSuccess)
            return TypedResults.Created("", result.Success!);

        _logger.LogErrorList(result.Error!);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: "Error while creating Product.",
            detail: result.Error!.ToDetailMessage(),
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}