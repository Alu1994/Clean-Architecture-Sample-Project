using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;

public static partial class UserEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}/create", async (IUserRepository userRepository, CreateUserRequest request, CancellationToken cancellation) =>
        {
            var user = await CreateUser(userRepository, request, cancellation);
            return user;
        })
        .Produces<CreateUserResponse>(Created, ContentType)
        .WithConfigSummaryInfo("Create User", TagName);

        return app;
    }

    private static async Task<CreateUserResponse> CreateUser(IUserRepository userRepository, CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = userRequest.Name,
            Email = userRequest.Email,
            Password = userRequest.Password,
            CreationDate = userRequest.CreationDate
        };
        var isSuccess = await userRepository.Insert(user, cancellationToken);
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
