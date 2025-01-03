﻿using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using CleanArchitectureSampleProject.Infrastructure.Messaging;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Cache;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using static CleanArchitectureSampleProject.Aspire.Configurations.AspireConfigurations;

namespace CleanArchitectureSampleProject.Infrastructure.Repository;

public static class DependencyInjection
{
    public static IHostApplicationBuilder BuildRepositoryMigration(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.AddNpgsqlDbContext<ProductDataContextMigration>(Services.PostgresDatabaseName);
        }
        else
        {
            // Is not local env (Not .NET Aspire)
            builder.Services.AddDbContext<ProductDataContextMigration>(options =>
                options.UseNpgsql("Host=localhost;Port=5441;Username=postgres;Password=dzJfU-Xv1rBzmckyTm05Cg;Database=dbproducts"));
        }

        return builder;
    }

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

        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
        //    .AddSingleton<IProductRepository, ProductRepositoryCache>();

        // ============================ PRODUCT ============================
        services
            .AddScoped<ICategoryRepository, CategoryRepositoryPostgres>()
            .AddScoped<IProductRepository, ProductRepositoryPostgres>();

        services
            .AddScoped<ICategoryRepositoryDatabase, CategoryRepositoryPostgres>()
            .AddScoped<IProductRepositoryDatabase, ProductRepositoryPostgres>();

        services
            .AddScoped<ICategoryRepositoryCache, CategoryRepositoryCache>()
            .AddScoped<IProductRepositoryCache, ProductRepositoryCache>();
        // ============================ PRODUCT ============================

        // ============================ SELL ============================
        services
            .AddScoped<ISellItemRepository, SellItemRepositoryPostgres>()
            .AddScoped<ISellRepository, SellRepositoryPostgres>();

        services
            .AddScoped<ISellItemRepositoryDatabase, SellItemRepositoryPostgres>()
            .AddScoped<ISellRepositoryDatabase, SellRepositoryPostgres>();

        services
            .AddScoped<ISellItemRepositoryCache, SellItemRepositoryCache>()
            .AddScoped<ISellRepositoryCache, SellRepositoryCache>();
        // ============================ SELL ============================

        return services;
    }

    public static IServiceCollection AddCacheAutoRefresh(this IServiceCollection services)
    {
        return services.AddHostedService<LoadCacheBackgroundService>();
    }
}
