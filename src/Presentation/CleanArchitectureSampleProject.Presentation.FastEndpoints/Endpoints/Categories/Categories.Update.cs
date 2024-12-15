using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;

public sealed class UpdateCategories(ILogger<UpdateCategories> logger, ICategoryUseCases categoryUseCases) :
    Endpoint<UpdateCategoryInput, Results<Ok<CategoryOutput>, ProblemHttpResult>>
{
    private readonly ILogger<UpdateCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Put("/categories");
        Description(b => b
            .Accepts<UpdateCategoryInput>(false, "application/json")
            .Produces<CategoryOutput>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(CategoryCanWritePolicy));
        Validator<UpdateCategoryInputValidator>();
    }

    public override async Task<Results<Ok<CategoryOutput>, ProblemHttpResult>> ExecuteAsync(UpdateCategoryInput category, CancellationToken cancellation)
    {
        var result = await _categoryUseCases.UpdateCategory(category, cancellation);

        if (result.IsSuccess)
            return TypedResults.Ok(result.Success!);

        _logger.LogError(message: result.Error!.Message);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: result.Error!.Message,
            detail: result.Error!.Message,
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}