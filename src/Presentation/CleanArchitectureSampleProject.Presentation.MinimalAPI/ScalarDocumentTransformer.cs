using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI;

public static class ScalarOpenApi
{
    public static void Configure(OpenApiOptions options)
    {
        options.AddDocumentTransformer(new ScalarDocumentTransformer());
    }

    public static void SetupOptions(ScalarOptions options)
    {
        options
            .WithTitle("Minimal API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }
}

public class ScalarDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers =
        [
            new() { Url = "https://localhost:7435" }
        ];
        return Task.CompletedTask;
    }
}
