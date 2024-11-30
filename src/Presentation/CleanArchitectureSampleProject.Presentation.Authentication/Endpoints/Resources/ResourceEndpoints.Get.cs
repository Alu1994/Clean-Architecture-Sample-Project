using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Resources;
using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities.Users;
using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Endpoints.Resources;

public static partial class ResourceEndpoints
{
    private static WebApplication MapGetByName(this WebApplication app)
    {
        app.MapGet($"/{Controller}/{{name}}", async (IResourceRepository resourceRepository, string name, CancellationToken cancellation) =>
        {
            var result = await GetResourceByName(resourceRepository, name, cancellation);
            return result;
        })
        .Produces<CreateResourceResponse>(Success, ContentType)
        .WithConfigSummaryInfo("Get Resource By Name", TagName);

        return app;
    }

    private static async Task<IResult> GetResourceByName(IResourceRepository resourceRepository, string name, CancellationToken cancellation)
    {
        var resourceResponse = await resourceRepository.GetResourceByName(name, cancellation);
        if (resourceResponse.IsFail) return resourceResponse.Error!.ToProblemDetails();
        var resource = resourceResponse.Success!;
        return Results.Ok(new CreateResourceResponse
        {
            Id = resource.Id,
            Name = resource.Name
        });
    }
}
