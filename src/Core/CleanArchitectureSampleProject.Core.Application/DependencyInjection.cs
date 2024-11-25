using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureSampleProject.Core.Application.UseCases;

namespace CleanArchitectureSampleProject.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        return services
            .AddScoped<IProductUseCases, ProductUseCases>()
            .AddScoped<ICategoryUseCases, CategoryUseCases>();
    }
}
