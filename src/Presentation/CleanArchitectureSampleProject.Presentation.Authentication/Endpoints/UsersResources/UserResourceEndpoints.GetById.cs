using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    private static WebApplication MapGetById(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/{{id:int}}", async (IUserResourceRepository userResourceRepository, int id, CancellationToken cancellation) =>
        {
            var result = await GetUserById(userResourceRepository, id, cancellation);
            return result;
        })
        .Produces<IReadOnlyCollection<UserResourceView>>(Success, ContentType)
        .WithConfigSummaryInfo("Get Complete Users Resources View By Id", TagName);

        return app;
    }

    private static async Task<IResult> GetUserById(IUserResourceRepository userResourceRepository, int id, CancellationToken cancellation)
    {
        var result = await userResourceRepository.GetCompleteUsersResourcesBy(x => x.UserId == id, cancellation);
        if (result.IsFail) 
            return result.Error!.ToProblemDetails();
        if(result.Success is { Count: 0 })
            return new BaseError($"No UserResource was found for User '{id}'.").ToProblemDetails();
        return Results.Ok(result.Success!.ToArray());
    }
}
