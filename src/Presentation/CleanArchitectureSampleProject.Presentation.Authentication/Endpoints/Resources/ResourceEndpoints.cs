namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Resources;

public static partial class ResourceEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Resources";
    private const string Controller = "resource";

    public static WebApplication MapResources(this WebApplication app)
    {
        app.MapCreate();
        app.MapGetByName();

        return app;
    }
}
