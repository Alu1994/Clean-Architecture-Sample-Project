using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;

public static partial class UserEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/{{name}}", async (IUserRepository userRepository, string name, CancellationToken cancellation) =>
        {
            var result = await GetUserByName(userRepository, name, cancellation);
            return result;
        })
        .Produces<CreateUserResponse>(Success, ContentType)
        .WithConfigSummaryInfo("Get User By Name", TagName);

        return app;
    }

    private static async Task<IResult> GetUserByName(IUserRepository userRepository, string name, CancellationToken cancellation)
    {
        var userResponse = await userRepository.GetUserByName(name, cancellation);
        if (userResponse.IsFail) return userResponse.Error!.ToProblemDetails();
        var user = userResponse.Success!;
        return Results.Ok(new CreateUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = "********",
            CreationDate = user.CreationDate
        });
    }
}
