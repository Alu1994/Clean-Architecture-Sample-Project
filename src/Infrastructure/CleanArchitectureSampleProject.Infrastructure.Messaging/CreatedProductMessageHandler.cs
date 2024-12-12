using Azure.Storage.Queues;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using MassTransit;
using Microsoft.Extensions.Logging;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public sealed class CreatedProductMessageHandler : IConsumer<CreateProductEvent>
{
    private readonly ILogger<CreatedProductMessageHandler> _logger;
    private readonly QueueServiceClient _queueServiceClient;
    private readonly QueueClient _queueClient;

    public CreatedProductMessageHandler(ILogger<CreatedProductMessageHandler> logger, QueueServiceClient queueServiceClient)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(Services.AzureQueueName);
        _queueClient.CreateIfNotExists();
    }

    public async Task Consume(ConsumeContext<CreateProductEvent> context)
    {
        _logger.LogInformation("{Event} was fired.", nameof(CreateProductEvent));
        await _queueClient.CreateIfNotExistsAsync();
        var jsonMessage = Json.SerializeWithoutReferenceLoop(new SerializeMessage<CreateProductEvent>(context.Message));
        await _queueClient.SendMessageAsync(jsonMessage);
    }
}
