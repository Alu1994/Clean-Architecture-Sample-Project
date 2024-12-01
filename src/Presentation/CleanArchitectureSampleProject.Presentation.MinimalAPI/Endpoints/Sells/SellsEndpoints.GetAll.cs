using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static RouteGroupBuilder MapGetAll(this RouteGroupBuilder app)
    {
        app.MapGet("", async (ILogger<Logging> logger, ISellUseCases sellUseCases, CancellationToken cancellation) =>
        {
            return await GetAll(logger, sellUseCases, cancellation);
        })
        .Produces<FrozenSet<GetSellOutput>>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get All Sells", TagName)
        //.RequireAuthorization(SellCanReadPolicy)
        ;

        return app;
    }

    private static async Task<IResult> GetAll(ILogger<Logging> logger, ISellUseCases sellUseCases, CancellationToken cancellation)
    {
        var result = await sellUseCases.GetSells(cancellation);
        return result.ToOkOrErrorResult(logger, "Error");
    }

}
