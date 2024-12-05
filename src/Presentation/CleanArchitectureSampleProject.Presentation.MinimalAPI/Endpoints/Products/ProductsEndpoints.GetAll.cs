using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static RouteGroupBuilder MapGetAll(this RouteGroupBuilder app)
    {
        app.MapGet("", async (ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation) =>
        {
            return await GetAll(logger, productUseCases, cancellation);
        })
        .Produces<FrozenSet<GetProductOutput>>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get All Products", TagName)
        .RequireAuthorization(ProductCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, IProductUseCases productUseCases, CancellationToken cancellation)
    {
        var result = await productUseCases.GetProducts(cancellation);
        return result.ToOkOrErrorResult(logger, "Error");
    }
}
