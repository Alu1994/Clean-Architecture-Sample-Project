using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;

public static partial class UserEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/user/{{name}}", async (IUserRepository userRepository, string name, CancellationToken cancellation) =>
        {
            var user = await GetUserByName(userRepository, name, cancellation);
            if (user.IsFail) return Results.BadRequest(user.Error);
            return Results.Ok(user.Success!);
        })
        .Produces<CreateUserResponse>(Success, ContentType)
        .WithConfigSummaryInfo("Get User By Name", TagName);

        return app;
    }

    private static async Task<Results<CreateUserResponse, BaseError>> GetUserByName(IUserRepository userRepository, string name, CancellationToken cancellation)
    {
        var userResponse = await userRepository.GetUserByName(name, cancellation);
        if (userResponse.IsFail) return userResponse.Error!;
        var user = userResponse.Success!;
        return new CreateUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = "********",
            CreationDate = user.CreationDate
        };
    }
}
