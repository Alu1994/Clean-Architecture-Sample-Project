using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Infrastructure.Repository;

public sealed class LoadCacheBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadCacheBackgroundService> _logger;

    public LoadCacheBackgroundService(IServiceProvider serviceProvider, ILogger<LoadCacheBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (true)
            {
                _logger.LogInformation("Start Loading Cache.");

                using var scope = _serviceProvider.CreateScope();
                {
                    await LoadCategories(scope, stoppingToken);
                    await LoadProducts(scope, stoppingToken);
                }

                _logger.LogInformation("Finish Loading Cache.");

                await Task.Yield();
                await Task.Delay(TimeSpan.FromMinutes(1));
                await Task.Yield();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while Loading Cache.");
        }
    }

    private static async Task LoadCategories(IServiceScope scope, CancellationToken stoppingToken)
    {
        var categoryRepository = scope.ServiceProvider.GetService<ICategoryRepositoryDatabase>()!;
        var categoryRepositoryCache = scope.ServiceProvider.GetService<ICategoryRepositoryCache>()!;
        var categoriesResult = await categoryRepository.Get(true, stoppingToken);
        await categoriesResult.MatchAsync<Validation<Error, FrozenSet<Category>>>(async categories =>
        {
            await categoryRepositoryCache.InsertAll(categories, stoppingToken);
            return categories;
        }, e => e);
    }

    private static async Task LoadProducts(IServiceScope scope, CancellationToken stoppingToken)
    {
        var productRepository = scope.ServiceProvider.GetService<IProductRepositoryDatabase>()!;
        var productRepositoryCache = scope.ServiceProvider.GetService<IProductRepositoryCache>()!;
        var productsResult = await productRepository.Get(true, stoppingToken);
        await productsResult.MatchAsync<Validation<Error, FrozenSet<Product>>>(async products =>
        {
            await productRepositoryCache.InsertAll(products, stoppingToken);
            return products;
        }, e => e);
    }
}