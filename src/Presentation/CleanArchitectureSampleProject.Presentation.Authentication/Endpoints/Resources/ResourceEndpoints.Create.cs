using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Resources;

public static partial class ResourceEndpoints
{
    private static WebApplication MapCreate(this WebApplication app)
    {
        app.MapPost($"/{Controller}/create", async (IResourceRepository resourceRepository, CreateResourceRequest request, CancellationToken cancellation) =>
        {
            var resource = await CreateResource(resourceRepository, request, cancellation);
            return resource;
        })
        .Produces<CreateResourceResponse>(Created, ContentType)
        .WithConfigSummaryInfo("Create Resource", TagName);

        return app;
    }

    private static async Task<CreateResourceResponse> CreateResource(IResourceRepository resourceRepository, CreateResourceRequest resourceRequest, CancellationToken cancellationToken)
    {
        var resource = new Resource
        {
            Name = resourceRequest.Name
        };
        var isSuccess = await resourceRepository.Insert(resource, cancellationToken);
        return new CreateResourceResponse
        {
            Id = resource.Id,
            Name = resource.Name
        };
    }
}
