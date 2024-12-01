namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    private static RouteGroupBuilder MapUpdate(this RouteGroupBuilder app)
    {
        app.MapPut("", async (ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation) =>
        {
            return await Update(logger, productUseCases, product, cancellation);
        })
        .Accepts<UpdateProductInput>(DefaultContentType)
        .Produces<UpdateProductOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Update Product", TagName)
        .RequireAuthorization(ProductCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, IProductUseCases productUseCases, UpdateProductInput product, CancellationToken cancellation)
    {
        var result = await productUseCases.UpdateProduct(product, cancellation);
        return result.ToOkOrErrorsResult(logger, "Error while updating new product.");
    }
}
