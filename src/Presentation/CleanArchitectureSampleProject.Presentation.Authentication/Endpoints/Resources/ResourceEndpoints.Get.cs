using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Resources;

public static partial class ResourceEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/{{name}}", async (IResourceRepository resourceRepository, string name, CancellationToken cancellation) =>
        {
            var user = await GetResourceByName(resourceRepository, name, cancellation);

            if (user.IsFail) return Results.BadRequest(user.Error);

            return Results.Ok(user.Success!);
        })
        .Produces<CreateResourceResponse>(Success, ContentType)
        .WithConfigSummaryInfo("Get Resource By Name", TagName);

        return app;
    }

    private static async Task<Results<CreateResourceResponse, BaseError>> GetResourceByName(IResourceRepository resourceRepository, string name, CancellationToken cancellation)
    {
        var resourceResponse = await resourceRepository.GetResourceByName(name, cancellation);
        if (resourceResponse.IsFail) return resourceResponse.Error!;
        var resource = resourceResponse.Success!;

        return new CreateResourceResponse
        {
            Id = resource.Id,
            Name = resource.Name
        };
    }
}
