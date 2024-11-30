using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapUpdate(this WebApplication app)
    {
        app.MapPut($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation) =>
        {
            return await Update(logger, productUseCases, product, cancellation);
        })
        .Accepts<UpdateProductInput>(ContentType)
        .Produces<UpdateProductOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Update Product", TagName)
        .AddFluentValidationAutoValidation()
        .RequireAuthorization(ProductCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation)
    {
        const string errorTitle = "Error while updating new product.";

        var result = await productUseCases.UpdateProduct(product, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogErrorList(error);

                return Results.BadRequest(new
                {
                    @Type = HttpStatusCode.BadRequest.ToString(),
                    Title = errorTitle,
                    Detail = errorTitle,
                    Errors = error.Errors.Select(x => x.Message),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        );
    }
}

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductInput>
{
    public UpdateProductValidator()
    {
        RuleFor(product => product.Id).NotEmpty();
        RuleFor(product => product.ToProduct(null))
            .SetValidator(new ProductValidator());
    }
}