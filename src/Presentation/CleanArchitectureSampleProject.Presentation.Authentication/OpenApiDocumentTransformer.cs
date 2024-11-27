using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers.Clear();
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