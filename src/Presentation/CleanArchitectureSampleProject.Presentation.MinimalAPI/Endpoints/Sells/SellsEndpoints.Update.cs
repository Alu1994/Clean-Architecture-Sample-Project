using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static RouteGroupBuilder MapUpdate(this RouteGroupBuilder app)
    {
        app.MapPut("", async (ILogger<Logging> logger, ISellUseCases sellUseCases, UpdateSellInput sell, CancellationToken cancellation) =>
        {
            return await Update(logger, sellUseCases, sell, cancellation);
        })
        .Accepts<UpdateSellInput>(DefaultContentType)
        .Produces<UpdateSellOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Update Sell", TagName)
        .RequireAuthorization(SellCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Update(ILogger<Logging> logger, ISellUseCases sellUseCases, UpdateSellInput sell, CancellationToken cancellation)
    {
        var result = await sellUseCases.UpdateSell(sell, cancellation);
        return result.ToOkOrErrorsResult(logger, "Error while updating new sell.");
    }
}