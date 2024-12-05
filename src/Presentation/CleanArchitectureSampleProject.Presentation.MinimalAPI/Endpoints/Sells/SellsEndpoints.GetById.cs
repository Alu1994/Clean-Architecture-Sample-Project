namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Endpoints.Products;

public static partial class SellsEndpoints
{
    private static RouteGroupBuilder MapGetById(this RouteGroupBuilder app)
    {
        app.MapGet("/{sellId:Guid}", async (ILogger<Logging> logger, ISellUseCases sellUseCases, Guid sellId, CancellationToken cancellation) =>
        {
            return await GetById(logger, sellUseCases, sellId, cancellation);
        })
        .Produces<GetSellOutput>(Success, DefaultContentType)
        .Produces<ProblemDetails>(BadRequest, DefaultContentType)
        .WithConfigSummaryInfo("Get Sell By Id", TagName)
        .RequireAuthorization(SellCanReadPolicy);

        return app;
    }

    private static async Task<IResult> GetById(ILogger<Logging> logger, ISellUseCases sellUseCases, Guid sellId, CancellationToken cancellation)
    {
        var result = await sellUseCases.GetSellById(sellId, cancellation);
        return result.ToOkOrErrorResult(logger, "Error while getting sell by id.");
    }
}
