using CleanArchitectureSampleProject.Presentation.Authentication.Messages;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Setups;

public static class FluentValidatorSetup
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        services.AddFluentValidationAutoValidation(configuration =>
        {
            // Replace the default result factory with a custom implementation.
            configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
        });

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

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var validationProblemErrors = validationResult.ToValidationProblemErrors();

        return Results.ValidationProblem(validationProblemErrors, "Error while validating request.", "Error while validating request.", (int)HttpStatusCode.BadRequest, "Error while validating request.");
    }
}