namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Users Resources";
    private const string Controller = "user-resource";

    public static WebApplication MapUsersResources(this WebApplication app)
    {
        app.MapCreate()
           .MapGetById()
           .MapGetByName();

        return app;
    }
}
