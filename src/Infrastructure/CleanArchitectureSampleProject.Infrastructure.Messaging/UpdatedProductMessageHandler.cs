using Azure.Storage.Queues;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using MassTransit;
using Microsoft.Extensions.Logging;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public sealed class UpdatedProductMessageHandler : IConsumer<UpdateProductEvent>
{
    private readonly ILogger<UpdatedProductMessageHandler> _logger;
    private readonly QueueServiceClient _queueServiceClient;
    private readonly QueueClient _queueClient;

    public UpdatedProductMessageHandler(ILogger<UpdatedProductMessageHandler> logger, QueueServiceClient queueServiceClient)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(Services.AzureQueueName);
        _queueClient.CreateIfNotExists();
    }

    public async Task Consume(ConsumeContext<UpdateProductEvent> context)
    {
        _logger.LogInformation("{Event} was fired.", nameof(UpdateProductEvent));
        await _queueClient.CreateIfNotExistsAsync();
        var jsonMessage = Json.SerializeWithoutReferenceLoop(context.Message.ProductId);
        await _queueClient.SendMessageAsync(jsonMessage);
    }
}