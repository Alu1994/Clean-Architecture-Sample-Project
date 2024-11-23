using CleanArchitectureSampleProject.Application.Inputs;
using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.Application.UseCases;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Categories;

public static partial class CategoriesEndpoints
{
    private static WebApplication MapUpdate(this WebApplication app)
    {
        app.MapPut($"/{Controller}", async (ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CategoryInput category, CancellationToken cancellation) =>
        {
            return await Update(logger, categoryUseCases, category, cancellation);
        })
        .Accepts<CategoryInput>(ContentType)
        .Produces<CategoryOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo($"Update {Controller}", TagName);
        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, ICategoryUseCases categoryUseCases, CategoryInput category, CancellationToken cancellation)
    {
        const string errorTitle = "Error while updating category.";

        var result = await categoryUseCases.UpdateCategory(category, cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogSeqError(error);
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
