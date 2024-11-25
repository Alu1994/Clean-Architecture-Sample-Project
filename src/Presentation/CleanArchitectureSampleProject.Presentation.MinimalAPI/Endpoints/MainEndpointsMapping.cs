﻿using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;

public static class MainEndpointsMapping
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapProducts();
        app.MapCategories();

        return app;
    }

    public static RouteHandlerBuilder WithConfigSummaryInfo(this RouteHandlerBuilder builder, string description, params string[] tagName)
    {
        return builder.WithName(description)
            .WithDescription(description)
            .WithSummary(description)
            .WithDisplayName(description)
            .WithTags(tagName)
            .WithOpenApi();
    }
}
