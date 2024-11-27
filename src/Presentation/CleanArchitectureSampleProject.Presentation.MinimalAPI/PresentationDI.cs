using NLog.Web;
using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints;
using Scalar.AspNetCore;
using CleanArchitectureSampleProject.Core.Domain;
using CleanArchitectureSampleProject.Core.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration.Scalar;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using CleanArchitectureSampleProject.Presentation.MinimalAPI.ApiDocsConfiguration.Swagger;
using Microsoft.Extensions.DependencyInjection;

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

        //services.AddSwaggerGen((x) => new ConfigureSwaggerOptions().Configure(x));
        //services.AddEndpointsApiExplorer();

        //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        

        // =========== Setup Authentication & Authorization ===========
        services.AddMyAuthentication();
        // =========== Setup Authentication & Authorization ===========

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

        // =========== Use Authentication & Authorization ===========
        app.UseAuthentication();
        app.UseAuthorization();
        // =========== Use Authentication & Authorization ===========

        // =========== Map Endpoints ===========
        app.UseHttpsRedirection();
        app.MapDefaultEndpoints();
        app.MapEndpoints();
        // =========== Map Endpoints ===========

        return app;
    }

    public static string MyPolicyName = "MyPolicyName";
    public static string myclaimname = "myclaimname";

    private static IServiceCollection AddMyAuthentication(this IServiceCollection services) 
    {
        services.AddAuthorization();
        //services.AddAuthorization(
        //    //options =>
        //    //    options.AddPolicy(MyPolicyName, p => 
        //    //        p.RequireClaim(myclaimname, "true"))
        //    );

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = 
                    new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey("MyTokenNeedsToHave256BytesForItToWorkAndBeSecure"u8.ToArray()),
                        ValidIssuer = "https://id.cleanarchsampleproject.com.br",
                        ValidAudience = "https://cleanarchsampleproject.com.br",
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                    };
            });

        return services;
    }
}
