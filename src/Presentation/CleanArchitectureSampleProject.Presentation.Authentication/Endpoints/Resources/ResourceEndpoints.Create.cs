using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;
using System.Net;

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

    private static async Task<IResult> CreateResource(IResourceRepository resourceRepository, CreateResourceRequest resourceRequest, CancellationToken cancellationToken)
    {
        var resource = new Resource
        {
            Name = resourceRequest.Name
        };
        var result = await resourceRepository.Insert(resource, cancellationToken);
        if (result.IsFail)
        {
            return Results.Problem(detail: result.Error.Message,
                statusCode: BadRequest,
                title: "Error while inserting new Resource.",
                type: HttpStatusCode.BadRequest.ToString());
        }

        return Results.Ok(new CreateResourceResponse
        {
            Id = resource.Id,
            Name = resource.Name
        });
    }
}
