using NLog.Web;
using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;
using Scalar.AspNetCore;
using CleanArchitectureSampleProject.Core.Domain;
using CleanArchitectureSampleProject.Core.Application;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI;

public static class PresentationDI
{
    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        builder.BuildRepository();

        // =========== Add NLog ===========
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Host.UseNLog();
        // =========== Add NLog ===========

        if (builder.Environment.IsDevelopment())
        {
            // =========== Add service defaults & Aspire client integrations. ===========
            // (This must be after 'builder.Logging.ClearProviders()' to re-add the LoggingProviders
            builder.AddServiceDefaults();
            // =========== Add service defaults & Aspire client integrations. ===========
        }
        else
        {
            // Is not local env (Not .NET Aspire)
        }

        return builder;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Problem Details
        services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi(ScalarOpenApi.Configure);
        services.AddSwaggerGen();
        services.AddEndpointsApiExplorer();
                
        // =========== Add Layers Dependency Injection ===========
        services.AddDomainLayer();
        services.AddApplicationLayer();
        services.AddRepositoryLayer();
        services.AddCacheAutoRefresh();
        // =========== Add Layers Dependency Injection ===========

        services.AddValidation();

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        // ======== Add Scalar UI ========
        app.MapScalarApiReference(ScalarOpenApi.SetupOptions);
        // ======== Add Scalar UI ========

        // ======== Add Swagger UI ========
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Test"));
        // ======== Add Swagger UI ========

        // =========== Map Endpoints ===========
        app.UseHttpsRedirection();
        app.MapDefaultEndpoints();
        app.MapEndpoints();
        // =========== Map Endpoints ===========

        return app;
    }
}
