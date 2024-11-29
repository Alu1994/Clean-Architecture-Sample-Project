using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/name/{{name}}", async (IUserResourceRepository userResourceRepository, string name, CancellationToken cancellation) =>
        {
            var user = await GetUserByName(userResourceRepository, name, cancellation);
            if (user.IsFail) return Results.BadRequest(user.Error);
            return Results.Ok(user.Success!);
        })
        .Produces<IReadOnlyCollection<UserResourceView>>(Success, ContentType)
        .WithConfigSummaryInfo("Get Complete Users Resources View By User Name", TagName);

        return app;
    }

    private static async Task<Results<IReadOnlyCollection<UserResourceView>, BaseError>> GetUserByName(IUserResourceRepository userResourceRepository, string name, CancellationToken cancellation)
    {
        var userResponse = await userResourceRepository.GetCompleteUsersResourcesBy(x => x.User.Name == name, cancellation);
        if (userResponse.IsFail) return userResponse.Error!;
        return userResponse.Success!.ToArray();
    }
}
