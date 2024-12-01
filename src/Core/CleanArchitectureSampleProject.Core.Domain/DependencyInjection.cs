using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Services;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Validators;
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

        services.AddScoped<ICreateSellService, CreateSellService>();

        services.AddDomainValidators();

        return services;
    }

    private static IServiceCollection AddDomainValidators(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssemblyContaining<SellValidator>()
            .AddValidatorsFromAssemblyContaining<SellItemValidator>()

            .AddValidatorsFromAssemblyContaining<ProductValidator>()
            .AddValidatorsFromAssemblyContaining<CategoryValidator>()
            .AddScoped<IGetOrCreateCategoryValidator, GetOrCreateCategoryValidator>();
        return services;
    }
}
