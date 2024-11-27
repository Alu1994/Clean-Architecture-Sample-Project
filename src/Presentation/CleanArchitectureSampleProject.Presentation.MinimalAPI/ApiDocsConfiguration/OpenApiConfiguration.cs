using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration;

public static class OpenApiConfiguration
{
    private const string Title = "Minimal API";

    public static void SetupScalarOptions(ScalarOptions options)
    {
        options
            .WithTitle(Title)
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }
}
