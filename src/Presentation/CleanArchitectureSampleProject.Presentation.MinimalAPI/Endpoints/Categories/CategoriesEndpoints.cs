namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Categories";
    private const string Controller = "category";
    private const string ContentType = "application/json";
    private const short Success = StatusCodes.Status200OK;
    private const short Created = StatusCodes.Status201Created;
    private const short BadRequest = StatusCodes.Status400BadRequest;
    private const short Unauthorized = StatusCodes.Status401Unauthorized;

    public static WebApplication MapCategories(this WebApplication app)
    {
        app.MapGetAll();
        app.MapGetById();
        app.MapGetByName();
        app.MapCreate();
        app.MapUpdate();
        return app;
    }
}
