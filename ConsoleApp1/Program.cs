using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

var builder = Host.CreateApplicationBuilder(args);

builder.AddAzureQueueClient(Services.AzureQueueName);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Listener>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Listener.ActivityName));

var host = builder.Build();
host.Run();
