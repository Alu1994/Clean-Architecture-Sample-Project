using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication;
using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Service.DatabaseMigration;

var builder = Host.CreateApplicationBuilder(args);

builder.BuildRepository().BuildAuthRepository();

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddRepositoryLayer().AddAuthRepositoryLayer();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivityName));

var host = builder.Build();
host.Run();
