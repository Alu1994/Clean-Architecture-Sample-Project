using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Infrastructure.Messaging;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Repository;

public static class DependencyInjection
{
    public static IHostApplicationBuilder BuildRepository(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            // =========== Add Redis Cache - Aspire ===========
            builder.AddRedisDistributedCache(Services.RedisCacheName);
            // =========== Add Redis Cache - Aspire ===========

            // =========== Add EF PostgresDB - Aspire ===========
            builder.AddNpgsqlDbContext<ProductDataContext>(Services.PostgresDatabaseName);
            // =========== Add EF PostgresDB - Aspire ===========
        }
        else
        {
            // Is not local env (Not .NET Aspire)
        }

        builder.Services.AddInfrastructureMessagingLayer(builder);

        return builder;
    }

    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryMemory>()
        //    .AddSingleton<IProductRepository, ProductRepositoryMemory>();

        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
        //    .AddSingleton<IProductRepository, ProductRepositoryCache>();


        services
            .AddScoped<ICategoryRepository, CategoryRepositoryPostgres>()
            .AddScoped<IProductRepository, ProductRepositoryPostgres>();

        services
            .AddScoped<ICategoryRepositoryDatabase, CategoryRepositoryPostgres>()
            .AddScoped<IProductRepositoryDatabase, ProductRepositoryPostgres>();

        services
            .AddScoped<ICategoryRepositoryCache, CategoryRepositoryCache>()
            .AddScoped<IProductRepositoryCache, ProductRepositoryCache>();

        return services;
    }

    public static IServiceCollection AddCacheAutoRefresh(this IServiceCollection services)
    {
        return services.AddHostedService<LoadCacheBackgroundService>();
    }
}
