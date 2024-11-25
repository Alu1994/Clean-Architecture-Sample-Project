using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureMessagingLayer(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        builder.AddAzureQueueClient(Services.AzureQueueConnection);
        services.AddKeyedSingleton<IMessagingHandler, ProductCreatedMessageHandler>("ProductCreatedMessage");
        services.AddSingleton<List<IMessagingHandler>>(x =>
        {
            Thread.Sleep(10000);
            var productMessageHandler = x.GetKeyedService<IMessagingHandler>("ProductCreatedMessage");
            return [productMessageHandler];
        });
        return services;
    }
}
