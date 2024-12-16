using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;
using Http = Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitectureSampleProject.Presentation.FastEndpoints.Endpoints.Sells;

public sealed class CreateSells(ILogger<CreateSells> logger, ISellUseCases sellUseCases) : 
    Endpoint<CreateSellInput, Http.Results<Http.Created<CreateSellOutput>, Http.ProblemHttpResult>>
{
    private readonly ILogger<CreateSells> _logger = logger;
    private readonly ISellUseCases _sellUseCases = sellUseCases;

    public override void Configure()
    {
        Post("/sells");
        Description(b => b
            .Accepts<CreateSellInput>(false, "application/json")
            .Produces<CreateSellOutput>(StatusCodes.Status201Created, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest, "application/json")
            .Produces<UnauthorizedResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ForbiddenResponse>(StatusCodes.Status403Forbidden, "application/json"));

        Policy(x => x.SetPolicyClaims(SellCanWritePolicy));
        Validator<CreateSellInputValidator>();
    }

    public override async Task<Http.Results<Http.Created<CreateSellOutput>, Http.ProblemHttpResult>> ExecuteAsync(CreateSellInput sell, CancellationToken cancellation)
    {
        var result = await _sellUseCases.CreateSell(sell, cancellation);

        if (result.IsSuccess)
            return TypedResults.Created("", result.Success!);

        _logger.LogErrorList(result.Error!);
        return TypedResults.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: "Error while creating Sell.",
            detail: result.Error!.ToDetailMessage(),
            statusCode: StatusCodes.Status400BadRequest
        );
    }
}