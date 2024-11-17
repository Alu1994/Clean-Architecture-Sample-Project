using CleanArchitectureSampleProject.Repository;
using CleanArchitectureSampleProject.Repository.Entities;
using CleanArchitectureSampleProject.Service.DatabaseMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddRepositoryLayer(builder);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivityName));

var host = builder.Build();
host.Run();
