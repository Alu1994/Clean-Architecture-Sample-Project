using Azure.Storage.Queues;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Infrastructure.Messaging;
using System.Text.Json;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Presentation.Worker;

public class MessageProcessorWorker : BackgroundService
{
    private readonly ILogger<MessageProcessorWorker> _logger;
    private readonly QueueServiceClient _queueServiceClient;
    private readonly QueueClient _queueClient;

    public MessageProcessorWorker(ILogger<MessageProcessorWorker> logger, QueueServiceClient queueServiceClient)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(Services.AzureQueueName);
        _queueClient.CreateIfNotExists();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Process(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task Process(CancellationToken stoppingToken)
    {        
        var response = await _queueClient.ReceiveMessageAsync(cancellationToken: stoppingToken);
        if (response?.Value?.Body is null) return;

        var body = response.Value.Body.ToObjectFromJson<DeserializeMessage>()!;
        var isFailProcessMessage = body.Type switch
        {
            nameof(CreateProductEvent) => CreateProductEventHandle(body.Content),
            nameof(UpdateProductEvent) => UpdateProductEventHandle(body.Content),
            _ => false
        } is false;
        if (isFailProcessMessage) return;
        await _queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt, cancellationToken: stoppingToken);
    }

    private bool CreateProductEventHandle(string content)
    {
        var createProductEvent = JsonSerializer.Deserialize<CreateProductEvent>(content);
        _logger.LogInformation("CreateProductEvent: {ProductId}", createProductEvent!.ProductId);
        return true;
    }

    private bool UpdateProductEventHandle(string content)
    {
        var updateProductEvent = JsonSerializer.Deserialize<UpdateProductEvent>(content);
        _logger.LogInformation("UpdateProductEvent: {ProductId} with {Quantity}", updateProductEvent!.ProductId, updateProductEvent!.Quantity);
        return true;
    }
}
