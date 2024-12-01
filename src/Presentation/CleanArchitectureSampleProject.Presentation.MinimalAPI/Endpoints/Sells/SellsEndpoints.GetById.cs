using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static WebApplication MapGetById(this WebApplication app)
    {
        app.MapGet($"/{Controller}/{{sellId:Guid}}", async (ILogger<Logging> logger, ISellRepositoryDatabase sellRepositoryDatabase, Guid sellId, CancellationToken cancellation) =>
        {
            return await GetById(logger, sellRepositoryDatabase, sellId, cancellation);
        })
        .Produces<GetProductOutput>(Success, ContentType)
        .Produces<ProblemDetails>(BadRequest, ContentType)
        .WithConfigSummaryInfo("Get Sell By Id", TagName)
        //.RequireAuthorization(SellCanReadPolicy)
        ;

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, ISellRepositoryDatabase sellRepositoryDatabase, Guid sellId, CancellationToken cancellation)
    {
        const string errorTitle = "Error while getting sell by id.";

        var result = await sellRepositoryDatabase.GetById(sellId, cancellation);
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
