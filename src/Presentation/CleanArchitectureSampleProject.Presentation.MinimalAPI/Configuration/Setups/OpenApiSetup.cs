using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Setups;

public static class OpenApiSetup
{
    private const string Title = "Minimal API";

    public static void SetupOpenApiOptions(OpenApiOptions options)
    {
        options.AddDocumentTransformer(new DocumentTransformerSetup());
    }

    public static void SetupScalarOptions(ScalarOptions options)
    {
        options
            .WithTitle(Title)
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }

    public static void SetupSwaggerOptions(SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Test");
        options.InjectStylesheet("/css/swagger-dark-theme.css");
    }
}
