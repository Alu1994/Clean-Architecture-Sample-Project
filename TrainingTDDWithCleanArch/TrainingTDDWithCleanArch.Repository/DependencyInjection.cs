using Microsoft.Extensions.DependencyInjection;
using TrainingTDDWithCleanArch.Domain.Interfaces;
using TrainingTDDWithCleanArch.Repository.Entities.Cache;
using TrainingTDDWithCleanArch.Repository.Entities.Memory;

namespace TrainingTDDWithCleanArch.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        services
            .AddSingleton<ICategoryRepository, CategoryRepository>()
            .AddSingleton<IProductRepository, ProductRepository>();

        //services
        //    .AddSingleton<ICategoryRepository, CategoryRepositoryCache>()
        //    .AddSingleton<IProductRepository, ProductRepositoryCache>();

        return services;
    }
}
