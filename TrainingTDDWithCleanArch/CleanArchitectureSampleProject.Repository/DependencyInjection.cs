using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureSampleProject.Domain.Interfaces.Repositories;
using CleanArchitectureSampleProject.Repository.Entities.Cache;
using CleanArchitectureSampleProject.Repository.Entities.Memory;

namespace CleanArchitectureSampleProject.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryMemory>()
        //    .AddSingleton<IProductRepository, ProductRepositoryMemory>();

        services
            .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
            .AddSingleton<IProductRepository, ProductRepositoryCache>();

        return services;
    }
}
