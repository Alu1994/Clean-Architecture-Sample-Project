using CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureSampleProject.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services)
    {
        services.AddScoped<ICreateProductService, CreateProductService>();
        services.AddScoped<IUpdateProductService, UpdateProductService>();
        services.AddScoped<ICreateCategoryService, CreateCategoryService>();
        services.AddScoped<IUpdateCategoryService, UpdateCategoryService>();
        services.AddScoped<ICategoryGetOrCreateService, CategoryGetOrCreateService>();

        return services;
    }
}
