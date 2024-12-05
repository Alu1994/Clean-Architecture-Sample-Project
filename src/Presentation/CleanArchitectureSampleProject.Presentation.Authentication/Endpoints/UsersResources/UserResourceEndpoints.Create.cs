using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.UsersResources;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.UsersResources;

public static partial class UserResourceEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}/create", async (IUserResourceRepository userResourceRepository, CreateUserResourceRequest request, CancellationToken cancellation) =>
        {
            var userResource = await CreateUserResource(userResourceRepository, request, cancellation);
            return userResource;
        })
        .Produces<CreateUserResourceResponse>(Created, ContentType)
        .WithConfigSummaryInfo("Create User Resource", TagName);

        return app;
    }

    private static async Task<IResult> CreateUserResource(IUserResourceRepository userResourceRepository, CreateUserResourceRequest userResourceRequest, CancellationToken cancellationToken)
    {
        var userResource = new UserResource
        {
            UserId = userResourceRequest.UserId,
            ResourceId = userResourceRequest.ResourceId,
            CanRead = userResourceRequest.CanRead,
            CanWrite = userResourceRequest.CanWrite,
            CanDelete = userResourceRequest.CanDelete,
        };
        var result = await userResourceRepository.Insert(userResource, cancellationToken);
        if (result.IsFail)
        {
            return Results.Problem(detail: result.Error.Message,
                statusCode: BadRequest,
                title: "Error while inserting new User Resource.",
                type: HttpStatusCode.BadRequest.ToString());
        }

        return Results.Ok(new CreateUserResourceResponse
        {
            Id = userResource.Id,
            UserId = userResource.UserId,
            ResourceId = userResource.ResourceId,
            CanRead = userResource.CanRead,
            CanWrite = userResource.CanWrite,
            CanDelete = userResource.CanDelete,
        });
    }
}
