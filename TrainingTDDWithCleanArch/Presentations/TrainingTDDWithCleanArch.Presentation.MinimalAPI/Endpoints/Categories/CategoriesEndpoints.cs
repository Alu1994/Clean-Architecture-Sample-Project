using Microsoft.AspNetCore.Mvc;
using System.Collections.Frozen;
using System.Net;
using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

namespace TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Categories;

public static class CategoriesEndpoints
{
    private const string TagName = "Categories";
    private const string Controller = "category";
    private const string ContentType = "application/json";
    private const int Success = StatusCodes.Status200OK;
    private const int BadRequest = StatusCodes.Status400BadRequest;

    public static WebApplication MapCategories(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ICategoryUseCases categoryUseCases, CancellationToken cancellation) =>
        {
            return await GetAllCategories(categoryUseCases, cancellation);
        })
        .Produces<FrozenSet<Category>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithName("Get All Categories")
        .WithDescription("Get All Categories")
        .WithSummary("Get All Categories")
        .WithDisplayName("Get All Categories")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapGet($"/{Controller}/{{categoryId:Guid}}", async (ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation) =>
        {
            return await GetById(categoryUseCases, categoryId, cancellation);
        })
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithName($"Get {Controller} By Id")
        .WithDescription($"Get {Controller} By Id")
        .WithSummary($"Get {Controller} By Id")
        .WithDisplayName($"Get {Controller} By Id")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapGet($"/{Controller}/by-name/{{categoryName}}", async (ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation) =>
        {
            return await GetByName(categoryUseCases, categoryName, cancellation);
        })
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithName($"Get {Controller} By Name")
        .WithDescription($"Get {Controller} By Name")
        .WithSummary($"Get {Controller} By Name")
        .WithDisplayName($"Get {Controller} By Name")
        .WithTags(TagName)
        .WithOpenApi();

        app.MapPost($"/{Controller}", async (ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation) =>
        {
            return await CreateCategory(categoryUseCases, category, cancellation);
        })
        .Accepts(typeof(CreateCategoryInput), ContentType)
        .Produces<Category>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithName($"Create {Controller}")
        .WithDescription($"Create {Controller}")
        .WithSummary($"Create {Controller}")
        .WithDisplayName($"Create {Controller}")
        .WithTags(TagName)
        .WithOpenApi();

        return app;
    }

    public static async Task<IResult> GetAllCategories(ICategoryUseCases categoryUseCases, CancellationToken cancellation)
    {
        const string errorMessage = "Error while getting all categories.";

        var result = await categoryUseCases.GetCategories(cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    public static async Task<IResult> GetById(ICategoryUseCases categoryUseCases, Guid categoryId, CancellationToken cancellation)
    {
        const string errorMessage = "Error while getting category by id.";

        var result = await categoryUseCases.GetCategoryById(categoryId, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    public static async Task<IResult> GetByName(ICategoryUseCases categoryUseCases, string categoryName, CancellationToken cancellation)
    {
        const string errorMessage = "Error while getting category by name.";

        var result = await categoryUseCases.GetCategoryByName(categoryName, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    public static async Task<IResult> CreateCategory(ICategoryUseCases categoryUseCases, CreateCategoryInput category, CancellationToken cancellation)
    {
        const string errorMessage = "Error while creating new category.";

        var result = await categoryUseCases.CreateCategory(category, cancellation);

        return result.Match(x =>
            Results.Ok(x),
            x => Results.Problem(
                type: HttpStatusCode.BadRequest.ToString(),
                title: errorMessage,
                detail: x.ToSeq().Head.Message,
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }
}
