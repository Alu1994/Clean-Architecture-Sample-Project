namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Products";

    public static WebApplication MapProducts(this WebApplication app)
    {
        var endpoints = app.MapGroup("/product");
        endpoints.MapGetAll();
        endpoints.MapGetById();
        endpoints.MapGetByName();
        endpoints.MapCreate();
        endpoints.MapUpdate();
        return app;
    }
}
