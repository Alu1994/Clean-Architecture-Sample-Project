using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration;

public sealed class DocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Servers =
        [
            new() { Url = "https://localhost:7435" }
        ];

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

        document.Components = new OpenApiComponents
        {
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
            //,Schemas = new Dictionary<string, OpenApiSchema>
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
            //}
        };
        return Task.CompletedTask;
    }
}
