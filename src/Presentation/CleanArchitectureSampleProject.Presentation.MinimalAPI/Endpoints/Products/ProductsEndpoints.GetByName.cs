namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static RouteGroupBuilder MapGetByName(this RouteGroupBuilder app)
    {
        app.MapGet("/by-name/{productName}", async (ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation) =>
        {
            return await GetByName(logger, productUseCases, productName, cancellation);
        })
        .Produces<GetProductOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get Product By Name", TagName)
        .RequireAuthorization(ProductCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetByName(ILogger<Logging> logger, IProductUseCases productUseCases, string productName, CancellationToken cancellation)
    {
        var result = await productUseCases.GetProductByName(productName, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting product by name.");
    }

}
