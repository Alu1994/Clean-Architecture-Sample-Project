using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    private static WebApplication MapGetById(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/{{id:int}}", async (IUserResourceRepository userResourceRepository, int id, CancellationToken cancellation) =>
        {
            var user = await GetUserByName(userResourceRepository, id, cancellation);
            if (user.IsFail) return Results.BadRequest(user.Error);
            return Results.Ok(user.Success!);
        })
        .Produces<IReadOnlyCollection<UserResourceView>>(Success, ContentType)
        .WithConfigSummaryInfo("Get Complete Users Resources View By Id", TagName);

        return app;
    }

    private static async Task<Results<IReadOnlyCollection<UserResourceView>, BaseError>> GetUserByName(IUserResourceRepository userResourceRepository, int id, CancellationToken cancellation)
    {
        var userResponse = await userResourceRepository.GetCompleteUsersResourcesBy(x => x.UserId == id, cancellation);
        if (userResponse.IsFail) return userResponse.Error!;
        return userResponse.Success!.ToArray();
    }
}
