using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CleanArchitectureSampleProject.Service.DatabaseMigration;

public class Worker : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<Worker> _logger;

    internal const string ActivityName = "MigrationService";
    private static readonly ActivitySource _activitySource = new(ActivityName);

    public Worker(IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<Worker> logger)
    {
        this.serviceProvider = serviceProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDataContext>();
            await dbContext.Database.MigrateAsync(stoppingToken);

            var dbAuthContext = scope.ServiceProvider.GetRequiredService<AuthenticationDataContext>();
            await dbAuthContext.Database.MigrateAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
        hostApplicationLifetime.StopApplication();
    }
}
