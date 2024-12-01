using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static RouteGroupBuilder MapCreate(this RouteGroupBuilder app)
    {
        app.MapPost("", async (ILogger<Logging> logger, ISellUseCases sellUseCases, CreateSellInput sell, CancellationToken cancellation) =>
        {
            return await Create(logger, sellUseCases, sell, cancellation);
        })
        .Accepts<CreateSellInput>(DefaultContentType)
        .Produces<CreateSellOutput>(Created, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Create Sell", TagName)
        .RequireAuthorization(SellCanWritePolicy);

        return app;
    }

    private static async Task<IResult> Create(ILogger<Logging> logger, ISellUseCases sellUseCases, CreateSellInput sell, CancellationToken cancellation)
    {
        var result = await sellUseCases.CreateSell(sell, cancellation);
        return result.ToOkOrErrorsResult(logger, "Error while creating new sell.");
    }

}
