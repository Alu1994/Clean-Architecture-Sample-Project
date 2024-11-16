using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureSampleProject.Application.UseCases;

namespace CleanArchitectureSampleProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        return services.AddSingleton<IProductUseCases, ProductUseCases>()
            .AddSingleton<ICategoryUseCases, CategoryUseCases>();
    }
}
