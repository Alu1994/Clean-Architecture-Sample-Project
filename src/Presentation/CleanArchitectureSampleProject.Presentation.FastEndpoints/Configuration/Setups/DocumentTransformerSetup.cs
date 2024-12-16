using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Setups;

public sealed class DocumentTransformerSetup : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info.Title = "FastEndpoints API | v1";

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
        };
        return Task.CompletedTask;
    }
}
