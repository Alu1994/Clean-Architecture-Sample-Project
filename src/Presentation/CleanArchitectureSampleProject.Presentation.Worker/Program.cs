using CleanArchitectureSampleProject.Infrastructure.Messaging;
using CleanArchitectureSampleProject.Presentation.Worker;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.BuildMessaging();

// =========== Add NLog ===========
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddNLog(builder.Configuration, new NLogProviderOptions() { LoggingConfigurationSectionName = "NLog" });
// =========== Add NLog ===========

builder.AddServiceDefaults();

builder.Services.AddHostedService<MessageProcessorWorker>();

builder.Services.AddMessagingLayer();

var host = builder.Build();
host.Run();
