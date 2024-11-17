using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;
using CleanArchitectureSampleProject.Repository.Entities.Postgres;
using CleanArchitectureSampleProject.Repository.Entities;
using Microsoft.Extensions.Hosting;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;
using CleanArchitectureSampleProject.Repository.Entities.Cache;

namespace CleanArchitectureSampleProject.Repository;

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
        return builder;
    }

    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryMemory>()
        //    .AddSingleton<IProductRepository, ProductRepositoryMemory>();

        services
            .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
            .AddSingleton<IProductRepository, ProductRepositoryCache>();


        //services
        //    .AddScoped<ICategoryRepository, CategoryRepositoryPostgres>()
        //    .AddScoped<IProductRepository, ProductRepositoryPostgres>();
        //services
        //    .AddScoped<ICategoryRepositoryCache, CategoryRepositoryCache>()
        //    .AddScoped<IProductRepositoryCache, ProductRepositoryCache>();

        return services;
    }
}
