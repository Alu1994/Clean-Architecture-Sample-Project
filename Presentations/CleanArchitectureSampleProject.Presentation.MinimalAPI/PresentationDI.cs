using NLog.Web;
using CleanArchitectureSampleProject.Application;
using CleanArchitectureSampleProject.Domain;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;
using CleanArchitectureSampleProject.Repository;
//using CleanArchitectureSampleProject.Aspire.Configurations;
using CleanArchitectureSampleProject.Repository.Entities;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI;

public static class PresentationDI
{
    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        // =========== Add Redis Cache - Aspire ===========
        builder.AddRedisDistributedCache("cache");
        // =========== Add Redis Cache - Aspire ===========
                
        // =========== Add NLog ===========
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Host.UseNLog();
        // =========== Add NLog ===========

        // =========== Add service defaults & Aspire client integrations. ===========
        // (This must be after 'builder.Logging.ClearProviders()' to re-add the LoggingProviders
        builder.AddServiceDefaults();
        // =========== Add service defaults & Aspire client integrations. ===========

        return builder;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Problem Details
        services.AddProblemDetails();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
                
        // =========== Add Layers Dependency Injection ===========
        services.AddDomainLayer();
        services.AddApplicationLayer();
        services.AddRepositoryLayer(builder);
        // =========== Add Layers Dependency Injection ===========

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapOpenApi();
        //if (app.Environment.IsDevelopment())
        //{
            
        //}

        app.UseHttpsRedirection();

        // =========== Map Endpoints ===========
        app.MapDefaultEndpoints();
        app.MapEndpoints();
        // =========== Map Endpoints ===========
        return app;
    }
}
