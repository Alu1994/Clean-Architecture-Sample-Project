using CleanArchitectureSampleProject.Core.Application.Inputs.Sells;
using CleanArchitectureSampleProject.Core.Application.Outputs.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Services;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Core.Application.UseCases;

public interface ISellUseCases
{
    Task<Results<FrozenSet<GetSellOutput>, BaseError>> GetSells(CancellationToken cancellation);
    Task<Results<GetSellOutput, BaseError>> GetSellById(Guid sellId, CancellationToken cancellation);
    Task<Results<CreateSellOutput, ErrorList>> CreateSell(CreateSellInput productInput, CancellationToken cancellation);
    Task<Results<UpdateSellOutput, ErrorList>> UpdateSell(UpdateSellInput sellInput, CancellationToken cancellation);
}

public sealed class SellUseCases(
    ILogger<SellUseCases> logger,
    ISellRepository sellRepository,
    ICreateSellService createSellService) : ISellUseCases
{
    private readonly ILogger<SellUseCases> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISellRepository _sellRepository = sellRepository ?? throw new ArgumentNullException(nameof(sellRepository));
    private readonly ICreateSellService _createSellService = createSellService ?? throw new ArgumentNullException(nameof(createSellService));

    public async Task<Results<FrozenSet<GetSellOutput>, BaseError>> GetSells(CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName}", nameof(GetSells));

        var result = await _sellRepository.Get(cancellation);
        return result.Match<Results<FrozenSet<GetSellOutput>, BaseError>>(
            sells =>
                sells.Select<Sell, GetSellOutput>(sell => sell).ToFrozenSet(),
            error => error);
    }

    public async Task<Results<GetSellOutput, BaseError>> GetSellById(Guid sellId, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {SellId}", nameof(GetSellById), sellId);

        var result = await _sellRepository.GetById(sellId, cancellation);
        return result.Match<Results<GetSellOutput, BaseError>>(
            sell => (GetSellOutput)sell,
            error => error);
    }

    public async Task<Results<CreateSellOutput, ErrorList>> CreateSell(CreateSellInput sellInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {SellInput}", nameof(CreateSell), sellInput);

        var sell = sellInput.ToSell();
        var result = await _createSellService.Execute(sell, cancellation);
        if (result.IsFail)
        {
            return result.ToErrorList();
        }
        return (CreateSellOutput)result.ToSuccess()!;
    }

    public async Task<Results<UpdateSellOutput, ErrorList>> UpdateSell(UpdateSellInput sellInput, CancellationToken cancellation)
    {
        _logger.LogInformation("Logging {MethodName} with {SellInput}", nameof(UpdateSell), sellInput);

        var sell = sellInput.ToSell();
        var result = await _createSellService.Execute(sell, cancellation);
        if (result.IsFail)
        {
            return result.ToErrorList();
        }
        return (UpdateSellOutput)result.ToSuccess()!;
    }
}
