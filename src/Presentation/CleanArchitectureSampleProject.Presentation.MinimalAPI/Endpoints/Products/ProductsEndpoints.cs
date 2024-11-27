namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class ProductsEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Products";
    private const string Controller = "product";
    private const string ContentType = "application/json";
    private const short Success = StatusCodes.Status200OK;
    private const short Created = StatusCodes.Status201Created;
    private const short BadRequest = StatusCodes.Status400BadRequest;
    private const short Unauthorized = StatusCodes.Status401Unauthorized;

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
