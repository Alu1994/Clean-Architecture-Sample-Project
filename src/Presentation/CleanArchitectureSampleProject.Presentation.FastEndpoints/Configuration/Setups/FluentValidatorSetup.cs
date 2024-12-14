using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Setups;

public static class FluentValidatorSetup
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateCategoryInput>, CreateCategoryInputValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateSellValidator>();

        return services;
    }

    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}