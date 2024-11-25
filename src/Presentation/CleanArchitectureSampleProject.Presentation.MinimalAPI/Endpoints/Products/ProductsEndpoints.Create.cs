using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}", async (ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation) =>
        {
            return await Create(logger, productUseCases, product, cancellation);
        })
        .Accepts<CreateProductInput>(ContentType)
        .Produces<CreateProductOutput>(Created, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Create Product", TagName)
        .AddFluentValidationAutoValidation();
        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation)
    {
        const string errorTitle = "Error while creating new product.";

        var result = await productUseCases.CreateProduct(product, cancellation);
        return result.Match(success => Results.Created($"/{Controller}", success),
            error =>
            {
                if (error.GetType() == typeof(BaseError))
                {
                    var errorMessage = logger.LogBaseError((BaseError)error);
                    return Results.Problem(
                        type: HttpStatusCode.BadRequest.ToString(),
                        title: errorTitle,
                        detail: errorMessage,
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                ErrorList errors = (ErrorList)error;
                return Results.BadRequest(new
                {
                    @Type = HttpStatusCode.BadRequest.ToString(),
                    Title = errorTitle,
                    Errors = errors.Errors.Select(x => x.Message),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        );
    }

}

public sealed class CreateProductValidator : AbstractValidator<CreateProductInput>
{
    public CreateProductValidator()
    {
        RuleFor(product => product.ToProduct(null))
            .SetValidator(new ProductValidator());
    }
}
