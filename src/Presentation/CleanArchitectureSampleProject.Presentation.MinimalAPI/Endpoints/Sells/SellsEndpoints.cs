namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Sells";

    public static WebApplication MapSells(this WebApplication app)
    {
        var endpoints = app.MapGroup("/sell");
        endpoints.MapGetAll();
        endpoints.MapGetById();
        endpoints.MapCreate();
        return app;
    }
}
