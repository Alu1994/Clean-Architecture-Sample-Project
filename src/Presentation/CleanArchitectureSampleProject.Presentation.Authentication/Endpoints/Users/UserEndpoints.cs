namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;

public static partial class UserEndpoints
{
    public readonly record struct Logging;
    private const string TagName = "Users";
    private const string Controller = "user";

    public static WebApplication MapUsers(this WebApplication app)
    {
        app.MapCreate()
           .MapGetByName();

        return app;
    }
}
