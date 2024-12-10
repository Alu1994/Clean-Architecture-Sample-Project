using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static IHostApplicationBuilder BuildMessaging(this IHostApplicationBuilder builder)
    {
        builder.AddAzureQueueClient(Services.AzureQueueConnection);
        return builder;
    }

    public static IServiceCollection AddMessagingLayer(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddEvents();
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static void AddEvents(this IBusRegistrationConfigurator busRegistration)
    {
        busRegistration.AddConsumer(typeof(ProductCreatedMessageMassTransitHandler));
    }
}
