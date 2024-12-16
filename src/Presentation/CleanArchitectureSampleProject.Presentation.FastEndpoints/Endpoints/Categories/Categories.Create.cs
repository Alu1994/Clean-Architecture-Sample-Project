using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.Outputs.Products;
using Http = Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Categories;

public sealed class CreateCategories(ILogger<CreateCategories> logger, ICategoryUseCases categoryUseCases) : 
    Endpoint<CreateCategoryInput, Http.Results<Http.Created<CategoryOutput>, Http.ProblemHttpResult>>
{
    private readonly ILogger<CreateCategories> _logger = logger;
    private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

    public override void Configure()
    {
        Post("/categories");
        Description(b => b
            .Accepts<CreateCategoryInput>(false, "application/json")
            .Produces<CategoryOutput>(StatusCodes.Status201Created, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(CategoryCanWritePolicy));
        Validator<CreateCategoryInputValidator>();
    }

    public override async Task<Http.Results<Http.Created<CategoryOutput>, Http.ProblemHttpResult>> ExecuteAsync(CreateCategoryInput category, CancellationToken cancellation)
    {
        var result = await _categoryUseCases.CreateCategory(category, cancellation);

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