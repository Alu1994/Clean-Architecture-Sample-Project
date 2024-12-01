namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Sells";
    private const string Controller = "sell";
    private const string ContentType = "application/json";
    private const short Success = StatusCodes.Status200OK;
    private const short Created = StatusCodes.Status201Created;
    private const short BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapSells(this WebApplication app)
    {
        app.MapGetAll();
        app.MapGetById();

        return app;
    }
}
