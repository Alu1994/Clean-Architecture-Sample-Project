using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static RouteGroupBuilder MapCreate(this RouteGroupBuilder app)
    {
        app.MapPost("", async (ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation) =>
        {
            return await Create(logger, productUseCases, product, cancellation);
        })
        .Accepts<CreateProductInput>(DefaultContentType)
        .Produces<CreateProductOutput>(Created, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Create Product", TagName)
        .RequireAuthorization(ProductCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, IProductUseCases productUseCases, CreateProductInput product, CancellationToken cancellation)
    {
        var result = await productUseCases.CreateProduct(product, cancellation);
        return result.ToCreatedOrErrorsResult(logger, "Error while creating new product.");
    }

}
