namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static RouteGroupBuilder MapGetById(this RouteGroupBuilder app)
    {
        app.MapGet("/{productId:Guid}", async (ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation) =>
        {
            return await GetById(logger, productUseCases, productId, cancellation);
        })
        .Produces<GetProductOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get Product By Id", TagName)
        .RequireAuthorization(ProductCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, IProductUseCases productUseCases, Guid productId, CancellationToken cancellation)
    {
        var result = await productUseCases.GetProductById(productId, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting product by id.");
    }
}
