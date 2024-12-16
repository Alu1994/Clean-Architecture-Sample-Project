using CleanArchitectureSampleProject.Core.Application.Outputs;
using Http = Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Sells;

public sealed class GetByIdSells(ILogger<GetByIdSells> logger, ISellUseCases sellUseCases) :
    EndpointWithoutRequest<Http.Results<Http.Ok<GetSellOutput>, Http.NoContent, Http.ProblemHttpResult>>
{
    private readonly ILogger<GetByIdSells> _logger = logger;
    private readonly ISellUseCases _sellUseCases = sellUseCases;

    public override void Configure()
    {
        Get("/sells/{sellId:Guid}");
        Description(b => b
            .Produces<GetSellOutput>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(SellCanReadPolicy));
    }

    public override async Task<Http.Results<Http.Ok<GetSellOutput>, Http.NoContent, Http.ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var sellId = Route<Guid>("sellId");
        var result = await _sellUseCases.GetSellById(sellId, cancellation);

        if (result.IsSuccess && result.Success is null)
            return TypedResults.NoContent();

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