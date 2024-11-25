namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Categories";
    private const string Controller = "category";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int Created = StatusCodes.Status201Created;
    private const int BadRequest = StatusCodes.Status400BadRequest;

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
