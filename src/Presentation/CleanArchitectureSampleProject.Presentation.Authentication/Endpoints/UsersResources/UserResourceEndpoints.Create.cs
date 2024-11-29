using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

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

    private static async Task<CreateUserResourceResponse> CreateUserResource(IUserResourceRepository userResourceRepository, CreateUserResourceRequest userResourceRequest, CancellationToken cancellationToken)
    {
        var userResource = new UserResource
        {
            UserId = userResourceRequest.UserId,
            ResourceId = userResourceRequest.ResourceId,
            CanRead = userResourceRequest.CanRead,
            CanWrite = userResourceRequest.CanWrite,
            CanDelete = userResourceRequest.CanDelete,
        };
        var isSuccess = await userResourceRepository.Insert(userResource, cancellationToken);
        return new CreateUserResourceResponse
        {
            Id = userResource.Id,
            UserId = userResource.UserId,
            ResourceId = userResource.ResourceId,
            CanRead = userResource.CanRead,
            CanWrite = userResource.CanWrite,
            CanDelete = userResource.CanDelete,
        };
    }
}
