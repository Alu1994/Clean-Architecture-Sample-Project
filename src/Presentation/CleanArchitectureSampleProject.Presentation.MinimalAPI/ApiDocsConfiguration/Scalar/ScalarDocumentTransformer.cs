using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration.Scalar;

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

        document.Components = new OpenApiComponents
        {
            //Schemas = new Dictionary<string, OpenApiSchema>
            //{
            //    // Add a reusable schema component
            //    ["MyCustomType"] = new OpenApiSchema
            //    {
            //        Type = "object",
            //        Properties = new Dictionary<string, OpenApiSchema>
            //        {
            //            ["id"] = new OpenApiSchema { Type = "string", Description = "Unique identifier" },
            //            ["name"] = new OpenApiSchema { Type = "string", Description = "Name of the entity" }
            //        },
            //        Required = new HashSet<string> { "id" }
            //    }
            //},
            //Parameters = new Dictionary<string, OpenApiParameter>
            //{
            //    // Add a reusable parameter
            //    ["X-Custom-Header"] = new OpenApiParameter
            //    {
            //        Name = "X-Custom-Header",
            //        In = ParameterLocation.Header,
            //        Required = false,
            //        Description = "Custom header for API requests",
            //        Schema = new OpenApiSchema { Type = "string" }
            //    }
            //}, 
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Description",
                    Name = "Name",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            }
        };

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
        return Task.CompletedTask;
    }
}
