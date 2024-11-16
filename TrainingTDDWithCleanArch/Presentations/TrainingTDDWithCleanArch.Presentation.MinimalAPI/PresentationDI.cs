using NLog.Web;
using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints;
using TrainingTDDWithCleanArch.Repository;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI;

public static class PresentationDI
{
    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        // =========== Add Redis Cache ===========
        builder.AddRedisDistributedCache("cache");
        // =========== Add Redis Cache ===========

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

    public static IServiceCollection AddPresentation(this IServiceCollection services)
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
        services.AddRepositoryLayer();
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
