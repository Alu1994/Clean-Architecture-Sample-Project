using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/name/{{name}}", async (IUserResourceRepository userResourceRepository, string name, CancellationToken cancellation) =>
        {
            var result = await GetUserByName(userResourceRepository, name, cancellation);
            return result;
        })
        .Produces<IReadOnlyCollection<UserResourceView>>(Success, ContentType)
        .WithConfigSummaryInfo("Get Complete Users Resources View By User Name", TagName);

        return app;
    }

    private static async Task<IResult> GetUserByName(IUserResourceRepository userResourceRepository, string name, CancellationToken cancellation)
    {
        var result = await userResourceRepository.GetCompleteUsersResourcesBy(x => x.User.Name == name, cancellation);
        if (result.IsFail) 
            return result.Error!.ToProblemDetails();
        if (result.Success is { Count: 0 })
            return new BaseError($"No UserResource was found for User '{name}'.").ToProblemDetails();
        return Results.Ok(result.Success!.ToArray());
    }
}
