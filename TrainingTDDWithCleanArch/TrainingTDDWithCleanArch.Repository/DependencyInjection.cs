using Microsoft.Extensions.DependencyInjection;
using TrainingTDDWithCleanArch.Domain.Interfaces;
using TrainingTDDWithCleanArch.Repository.Entities;

namespace TrainingTDDWithCleanArch.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        return services.AddSingleton<IProductRepository, ProductRepository>()
            .AddSingleton<ICategoryRepository, CategoryRepository>();
    }
}
