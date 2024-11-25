using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapUpdate(this WebApplication app)
    {
        app.MapPut($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, UpdateCategoryInput category, CancellationToken cancellation) =>
        {
            return await Update(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<UpdateCategoryInput>(ContentType)
        .Produces<CategoryOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Update {Controller}", TagName)
        .AddFluentValidationAutoValidation();
        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, UpdateCategoryInput category, CancellationToken cancellation)
    {
        const string errorTitle = "Error while updating category.";

        var result = await categoryUseCases.UpdateCategory(category, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogBaseError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitle,
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }
}

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}
