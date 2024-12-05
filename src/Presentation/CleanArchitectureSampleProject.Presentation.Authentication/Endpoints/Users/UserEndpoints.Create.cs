using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Users;

public static partial class UserEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}/create", async (IUserRepository userRepository, CreateUserRequest request, CancellationToken cancellation) =>
        {
            var result = await CreateUser(userRepository, request, cancellation);
            return result;
        })
        .Produces<CreateUserResponse>(Created, ContentType)
        .WithConfigSummaryInfo("Create User", TagName);

        return app;
    }

    private static async Task<IResult> CreateUser(IUserRepository userRepository, CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = userRequest.Name,
            Email = userRequest.Email,
            Password = userRequest.Password,
            CreationDate = userRequest.CreationDate
        };
        var result = await userRepository.Insert(user, cancellationToken);
        if (result.IsFail)
        {
            return Results.Problem(detail: result.Error.Message,
                statusCode: BadRequest,
                title: "Error while inserting new User.",
                type: HttpStatusCode.BadRequest.ToString());
        }

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
