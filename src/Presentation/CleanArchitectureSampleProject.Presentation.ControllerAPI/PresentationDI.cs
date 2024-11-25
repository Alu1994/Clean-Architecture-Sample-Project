using CleanArchitectureSampleProject.Core.Application;
using CleanArchitectureSampleProject.Core.Domain;
using CleanArchitectureSampleProject.Infrastructure.Repository;

namespace CleanArchitectureSampleProject.Presentation.ControllerAPI;

public static class PresentationDI
{
    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        builder.BuildRepository();
        return builder;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddDomainLayer();
        services.AddApplicationLayer();
        services.AddRepositoryLayer();
        //services.AddCacheAutoRefresh();
        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        return app;
    }
}
