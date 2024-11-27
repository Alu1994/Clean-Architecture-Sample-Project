using CleanArchitectureSampleProject.Core.Domain;
using CleanArchitectureSampleProject.Core.Application;
using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;
using NLog.Web;
using Scalar.AspNetCore;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Setups;

public static class DependencyInjection
{
    private static IWebHostEnvironment Env;

    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        Env = builder.Environment;

        builder.BuildRepository();

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
        services.AddOpenApi(options => options.AddDocumentTransformer(new DocumentTransformer()));

        // =========== Setup Authentication & Authorization ===========
        services.AddAuthenticationAndAuthorization();
        // =========== Setup Authentication & Authorization ===========

        // =========== Add Layers Dependency Injection ===========
        services.AddDomainLayer();
        services.AddApplicationLayer();
        services.AddRepositoryLayer();
        services.AddCacheAutoRefresh();
        // =========== Add Layers Dependency Injection ===========

        services.AddValidation();

        if (Env.IsDevelopment() is false)
            services.AddAppDefaultHealthChecks();

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        // ======== Add Scalar UI ========
        app.MapScalarApiReference(OpenApiConfiguration.SetupScalarOptions);
        // ======== Add Scalar UI ========

        // ======== Add Swagger UI ========
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/openapi/v1.json", "Test");
            //options.InjectStylesheet("/css/swagger-dark-theme.css");
        });
        // ======== Add Swagger UI ========

        app.UseMiddleware<JwtMiddleware>();

        // =========== Use Authentication & Authorization ===========
        app.UseAuthentication();
        app.UseAuthorization();
        // =========== Use Authentication & Authorization ===========

        

        // =========== Map Endpoints ===========
        app.UseHttpsRedirection();
        app.MapEndpoints();
        // =========== Map Endpoints ===========

        if (Env.IsDevelopment())
        {
            // =========== Add DefaultEndpoints From Aspire. ===========
            app.MapDefaultEndpoints();
            // =========== Add DefaultEndpoints From Aspire. ===========
        }
        else
        {
            // Is not local env (Not .NET Aspire)
            app.MapAppDefaultEndpoints();
        }

        app.UseStaticFiles();

        return app;
    }
}
