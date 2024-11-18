using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Diagnostics;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace ConsoleApp1;

public class Listener(QueueServiceClient client) : BackgroundService
{
    private readonly QueueServiceClient _client = client ?? throw new ArgumentNullException(nameof(client));

    internal const string ActivityName = "QueueListener";
    private static readonly ActivitySource _activitySource = new(ActivityName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Listener for process: Klaviyo.Feed.Job starts");
        try
        {
            while (true)
            {
                // Consume Queue
                var queue = _client.GetQueueClient(Services.AzureQueueName);
                await queue.CreateIfNotExistsAsync();
                var myMessageSent = new MyMessage { Id = 1, MeuTexto = "Meu Texto de Test." };
                var jsonMyMessage = JsonConvert.SerializeObject(myMessageSent);
                var messageSent = await queue.SendMessageAsync(jsonMyMessage, cancellationToken: stoppingToken);

                await Task.Delay(1000);

                var message = await queue.ReceiveMessageAsync(cancellationToken: stoppingToken);
                var messageContent = message.Value.Body;
                var myMessageReceived = messageContent.ToObjectFromJson<MyMessage>();

                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while trying to access azure queue: {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Listener for process: Klaviyo.Feed.Job ends.");
        }
    }
}


public class MyMessage
{
    public int Id { get; set; }
    public string MeuTexto { get; set; }
}