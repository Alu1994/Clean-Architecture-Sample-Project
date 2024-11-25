using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureSampleProject.Core.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainLayer(this IServiceCollection services)
    {
        services.AddScoped<ICreateProductService, CreateProductService>();
        services.AddScoped<IUpdateProductService, UpdateProductService>();
        services.AddScoped<ICreateCategoryService, CreateCategoryService>();
        services.AddScoped<IUpdateCategoryService, UpdateCategoryService>();
        services.AddScoped<ICategoryGetOrCreateService, CategoryGetOrCreateService>();

        services.AddDomainValidators();

        return services;
    }

    private static IServiceCollection AddDomainValidators(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssemblyContaining<ProductValidator>()
            .AddValidatorsFromAssemblyContaining<CategoryValidator>()
            .AddScoped<IGetOrCreateCategoryValidator, GetOrCreateCategoryValidator>();
        return services;
    }
}
