using CleanArchitectureSampleProject.Core.Application.Outputs;
using Http = Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Sells;

public sealed class GetAllSells(ILogger<GetAllSells> logger, ISellUseCases sellUseCases) : 
    EndpointWithoutRequest<Http.Results<Http.Ok<FrozenSet<GetSellOutput>>, Http.NoContent, Http.ProblemHttpResult>>
{
    private readonly ILogger<GetAllSells> _logger = logger;
    private readonly ISellUseCases _sellUseCases = sellUseCases;

    public override void Configure()
    {
        Get("/sells");
        Description(b => b
            .Produces<FrozenSet<GetSellOutput>>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(SellCanReadPolicy));
    }

    public override async Task<Http.Results<Http.Ok<FrozenSet<GetSellOutput>>, Http.NoContent, Http.ProblemHttpResult>> ExecuteAsync(CancellationToken cancellation)
    {
        var result = await _sellUseCases.GetSells(cancellation);

        if (result.IsSuccess && result.Success!.Count is 0)
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