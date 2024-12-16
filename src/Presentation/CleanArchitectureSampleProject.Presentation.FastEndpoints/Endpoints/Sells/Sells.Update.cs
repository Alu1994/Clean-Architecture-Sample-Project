using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;
using Http = Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Sells;

public sealed class UpdateSells(ILogger<UpdateSells> logger, ISellUseCases sellUseCases) :
    Endpoint<UpdateSellInput, Http.Results<Http.Ok<UpdateSellOutput>, Http.ProblemHttpResult>>
{
    private readonly ILogger<UpdateSells> _logger = logger;
    private readonly ISellUseCases _sellUseCases = sellUseCases;

    public override void Configure()
    {
        Put("/sells");
        Description(b => b
            .Accepts<UpdateSellInput>(false, "application/json")
            .Produces<UpdateSellOutput>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(SellCanWritePolicy));
        Validator<UpdateSellInputValidator>();
    }

    public override async Task<Http.Results<Http.Ok<UpdateSellOutput>, Http.ProblemHttpResult>> ExecuteAsync(UpdateSellInput sell, CancellationToken cancellation)
    {
        var result = await _sellUseCases.UpdateSell(sell, cancellation);

        if (result.IsSuccess)
            return TypedResults.Ok(result.Success!);

        _logger.LogErrorList(result.Error!);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: "Error while updating Sell.",
            detail: result.Error!.ToDetailMessage(),
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}