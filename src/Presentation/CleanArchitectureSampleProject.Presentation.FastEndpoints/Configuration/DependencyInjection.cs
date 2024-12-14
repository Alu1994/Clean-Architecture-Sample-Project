using CleanArchitectureSampleProject.Core.Domain;
using CleanArchitectureSampleProject.Core.Application;
using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Middlewares;
using CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Setups;
using CleanArchitectureSampleProject.Infrastructure.Messaging;
using FastEndpoints.Swagger;
using NLog.Web;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration;

public static class DependencyInjection
{
    private static IWebHostEnvironment Env;

    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        Env = builder.Environment;

        builder.BuildRepository();
        builder.BuildMessaging();

        // =========== Add NLog ===========
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Host.UseNLog();
        // =========== Add NLog ===========

        if (Env.IsDevelopment())
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
        services.AddOpenApi(OpenApiSetup.SetupOpenApiOptions);
        services.AddFastEndpoints(o =>
        {
            o.IncludeAbstractValidators = true;
        });
        services.SwaggerDocument(
           //o =>
           //{
           //    o.DocumentSettings = s =>
           //    {
           //        s.SchemaSettings.SchemaProcessors.Add(new MySchemaProcessor(builder.Services)); //pass down the service collection to the schema processor
           //    };
           //}
           );

        // =========== Setup Authentication & Authorization ===========
        services.AddAuthenticationAndAuthorization();
        // =========== Setup Authentication & Authorization ===========

        // =========== Add Layers Dependency Injection ===========
        services.AddDomainLayer();
        services.AddApplicationLayer();
        services.AddRepositoryLayer();
        services.AddMessagingLayer();
        services.AddCacheAutoRefresh();
        // =========== Add Layers Dependency Injection ===========

        services.AddValidation();
        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        app.UseMiddleware<JwtMiddleware>();

        // =========== Use Authentication & Authorization ===========
        app.UseAuthentication();
        app.UseAuthorization();
        // =========== Use Authentication & Authorization ===========

        // =========== Map Endpoints ===========
        app.UseHttpsRedirection();
        // =========== Map Endpoints ===========

        if (Env.IsDevelopment())
        {
            // =========== Add DefaultEndpoints From Aspire. ===========
            app.MapDefaultEndpoints();
            // =========== Add DefaultEndpoints From Aspire. ===========
        }

        app.UseSwaggerGen();

        app.UseFastEndpoints(c => 
        { 
            c.Errors.ProducesMetadataType = typeof(Microsoft.AspNetCore.Mvc.ProblemDetails);
            //c.Errors.ProducesMetadataType = typeof(global::FastEndpoints.ProblemDetails);
        });

        app.UseStaticFiles();

        return app;
    }
}
