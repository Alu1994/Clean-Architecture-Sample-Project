using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Setups;

public static class HealthCheckSetup
{
    public static IServiceCollection AddAppDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddNpgSql((sv) =>
            {
                var pdc = sv.GetService<ProductDataContext>();
                return pdc!.Database.GetConnectionString()!;
            })
            //.AddRedis("")
            ;

        return services;
    }

    public static WebApplication MapAppDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
