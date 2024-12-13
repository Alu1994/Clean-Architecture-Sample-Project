using Azure.Storage.Queues;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Infrastructure.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Presentation.Worker;

public class MessageProcessorWorker : BackgroundService
{
    private readonly ILogger<MessageProcessorWorker> _logger;
    private readonly QueueServiceClient _queueServiceClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly QueueClient _queueClient;

    public MessageProcessorWorker(ILogger<MessageProcessorWorker> logger, QueueServiceClient queueServiceClient,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
        _serviceProvider = serviceProvider;
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
            nameof(CreateProductEvent) => await CreateProductEventHandle(body.Content, stoppingToken),
            nameof(UpdateProductEvent) => await UpdateProductEventHandle(body.Content, stoppingToken),
            _ => false
        } is false;
        if (isFailProcessMessage) return;
        await _queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt, cancellationToken: stoppingToken);
    }

    private async ValueTask<bool> CreateProductEventHandle(string content, CancellationToken stoppingToken)
    {
        var createProductEvent = JsonSerializer.Deserialize<CreateProductEvent>(content);
        _logger.LogInformation("CreateProductEvent: {ProductId}", createProductEvent!.ProductId);
        return await ValueTask.FromResult(true);
    }

    private async ValueTask<bool> UpdateProductEventHandle(string content, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        var updateProductEvent = JsonSerializer.Deserialize<UpdateProductEvent>(content);
        _logger.LogInformation("UpdateProductEvent: {ProductId} with Subtract Quantity of: {Quantity}", updateProductEvent!.ProductId, updateProductEvent!.Quantity);

        var result = await repository.GetById(updateProductEvent.ProductId, stoppingToken);

        if (result.IsFail) return false;

        var product = result.Success!;
        product.SubtractFromStock(updateProductEvent.Quantity);

        var updateResult = await repository.Update(product, stoppingToken);

        if(updateResult != ValidationResult.Success) return false;

        return true;
    }
}










