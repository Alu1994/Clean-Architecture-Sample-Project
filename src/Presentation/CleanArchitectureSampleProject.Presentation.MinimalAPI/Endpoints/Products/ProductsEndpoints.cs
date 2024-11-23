namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Products";
    private const string Controller = "product";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int Created = StatusCodes.Status201Created;
    private const int BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapProducts(this WebApplication app)
    {
        app.MapGetAll();
        app.MapGetById();
        app.MapGetByName();
        app.MapCreate();
        app.MapUpdate();
        return app;
    }

}
