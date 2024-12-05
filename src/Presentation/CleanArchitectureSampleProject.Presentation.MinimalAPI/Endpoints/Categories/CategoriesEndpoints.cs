namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Categories";

    public static WebApplication MapCategories(this WebApplication app)
    {
        var endpoints = app.MapGroup("/category");
        endpoints.MapGetAll();
        endpoints.MapGetById();
        endpoints.MapGetByName();
        endpoints.MapCreate();
        endpoints.MapUpdate();
        return app;
    }
}
