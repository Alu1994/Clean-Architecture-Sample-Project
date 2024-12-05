using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Setups;

public sealed class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string Title = "Authentication API | v1";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers.Clear();

        document.Info.Title = Title;

        document.Servers.Add(new OpenApiServer 
        { 
            Url = "https://localhost:7276",
            Description = "Local https API"
        });

        document.Servers.Add(new OpenApiServer
        {
            Url = "http://localhost:5126",
            Description = "Local http API",            
        });

        return Task.CompletedTask;
    }
}