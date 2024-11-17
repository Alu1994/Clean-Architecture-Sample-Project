using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;
using CleanArchitectureSampleProject.Repository.Entities.Cache;
using CleanArchitectureSampleProject.Repository.Entities.Memory;
using CleanArchitectureSampleProject.Repository.Entities.Postgres;
using CleanArchitectureSampleProject.Repository.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CleanArchitectureSampleProject.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // =========== Add EF PostgresDB - Aspire ===========
        builder.AddNpgsqlDbContext<ProductDataContext>("dbproducts");
        // =========== Add EF PostgresDB - Aspire ===========

        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryMemory>()
        //    .AddSingleton<IProductRepository, ProductRepositoryMemory>();

        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
        //    .AddSingleton<IProductRepository, ProductRepositoryCache>();



        services
            .AddScoped<ICategoryRepository, CategoryRepositoryPostgres>()
            .AddScoped<IProductRepository, ProductRepositoryPostgres>();

        return services;
    }
}
