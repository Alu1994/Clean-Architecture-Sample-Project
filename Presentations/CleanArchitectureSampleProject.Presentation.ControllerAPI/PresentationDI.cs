namespace CleanArchitectureSampleProject.Presentation.ControllerAPI;

public static class PresentationDI
{
    public static WebApplicationBuilder BuildPresentation(this WebApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache("cache");
        return builder;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        return app;
    }
}
