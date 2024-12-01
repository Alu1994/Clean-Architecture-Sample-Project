using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static WebApplication MapGetAll(this WebApplication app)
    {
        app.MapGet($"/{Controller}", async (ILogger<Logging> logger, ISellRepositoryDatabase sellRepositoryDatabase, CancellationToken cancellation) =>
        {
            return await GetAll(logger, sellRepositoryDatabase, cancellation);
        })
        .Produces<FrozenSet<GetProductOutput>>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get All Sells", TagName)
        //.RequireAuthorization(SellCanReadPolicy)
        ;

        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, ISellRepositoryDatabase sellRepositoryDatabase, CancellationToken cancellation)
    {
        var result = await sellRepositoryDatabase.Get(cancellation);
        return result.Match(success => Results.Ok(success),
            error =>
            {
                var errorMessage = logger.LogBaseError(error);
                return Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: "Error",
                    detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
        );
    }

}
