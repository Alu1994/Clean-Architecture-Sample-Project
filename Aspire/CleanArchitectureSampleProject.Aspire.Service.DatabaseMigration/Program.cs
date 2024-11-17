using CleanArchitectureSampleProject.Repository;
using CleanArchitectureSampleProject.Service.DatabaseMigration;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddRepositoryLayer(builder);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivityName));

var host = builder.Build();
host.Run();
