﻿using CleanArchitectureSampleProject.Application;
using CleanArchitectureSampleProject.Domain;
using CleanArchitectureSampleProject.Repository;

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
        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        return app;
    }
}
