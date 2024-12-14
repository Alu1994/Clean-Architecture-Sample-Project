using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Setups;

public static class OpenApiSetup
{
    public static void SetupOpenApiOptions(OpenApiOptions options)
    {
        options.AddDocumentTransformer(new DocumentTransformerSetup());
    }

    public static void SetupSwaggerOptions(SwaggerUIOptions options)
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Test");
        options.InjectStylesheet("/css/swagger-dark-theme.css");
    }
}
