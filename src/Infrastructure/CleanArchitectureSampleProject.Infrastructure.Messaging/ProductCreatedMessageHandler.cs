using Azure.Storage.Queues;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;
using System.Text.Json;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public sealed class ProductCreatedMessageHandler : IMessagingHandler
{
    private readonly QueueServiceClient _queueServiceClient;
    private readonly QueueClient _queueClient;
    public Type[] Event => [typeof(CreateProductEvent), typeof(UpdateProductEvent)];

    public ProductCreatedMessageHandler(QueueServiceClient queueServiceClient)
    {
        _queueServiceClient = queueServiceClient;
        _queueClient = _queueServiceClient.GetQueueClient(Services.AzureQueueName);
        _queueClient.CreateIfNotExists();
    }

    public async Task<IMessage> GetMessage(CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync();
        var message = await _queueClient.ReceiveMessageAsync(cancellationToken: cancellationToken);
        if (message.Value is null) return new ProductEventData();
        return new ProductEventData().WithBody(message.Value.Body.ToString());
    }

    public async Task<TMessage?> GetMessage<TMessage>(CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync();
        var message = await _queueClient.ReceiveMessageAsync(cancellationToken: cancellationToken);
        if (message.Value is null) return default;
        return JsonSerializer.Deserialize<TMessage>(message.Value.Body)!;
    }

    public async Task<RemoveResult> RemoveMessage(CreateResult message, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync();
        var response = await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
        return new RemoveResult(response is { Status: >= 200 and <= 299 }, response.Status);
    }

    public async Task<CreateResult> SendMessage(IMessage message, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync();
        var jsonMessage = Json.SerializeWithoutReferenceLoop((message as ProductEventData).Body);
        var response = await _queueClient.SendMessageAsync(jsonMessage, cancellationToken: cancellationToken);
        return new CreateResult(response.Value.MessageId, response.Value.PopReceipt, response.Value.InsertionTime);
    }

    public async Task<CreateResult> SendMessageWithResult(IMessage message, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync();
        var jsonMessage = JsonSerializer.Serialize((message as ProductEventData).Body);
        var response = await _queueClient.SendMessageAsync(jsonMessage, cancellationToken: cancellationToken);
        return new CreateResult(response.Value.MessageId, response.Value.PopReceipt, response.Value.InsertionTime);
    }
}