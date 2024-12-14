using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration.Middlewares;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;

public class CreateCategories(ILogger<CreateCategories> logger, ICategoryUseCases categoryUseCases) : 
    Endpoint<CreateCategoryInput, Results<Created<CategoryOutput>, ProblemHttpResult>>
{
    private readonly ILogger<CreateCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Post("/categories");
        Description(b => b
            .Accepts<CreateCategoryInput>(false, "application/json")
            .Produces<CategoryOutput>(201, "application/json")
            .ProducesProblemDetails(400, "application/json")
            .Produces<UnauthorizedResponse>(401, "application/json")
            .Produces<ForbiddenResponse>(403, "application/json"));

        var claim = GetAuthClaim(CategoryCanWritePolicy);
        Policy(x => x.RequireClaim(claim.Name, claim.AcceptedValues));
        Validator<CreateCategoryInputValidator>();
    }

    public override async Task<Results<Created<CategoryOutput>, ProblemHttpResult>> ExecuteAsync(CreateCategoryInput category, CancellationToken cancellation)
    {
        var result = await _categoryUseCases.CreateCategory(new CategoryInput { CategoryName = category.CategoryName }, cancellation);

        if (result.IsSuccess)
            return TypedResults.Created("", result.Success!);

        _logger.LogError(message: result.Error!.Message);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: result.Error!.Message,
            detail: result.Error!.Message,
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}

public sealed class CreateCategoryInputValidator : AbstractValidator<CreateCategoryInput>
{
    public CreateCategoryInputValidator()
    {
        RuleFor(x => x.ToCategory()).SetValidator(new CategoryValidator());
    }
}