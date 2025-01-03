﻿using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Services;

public interface IUpdateSellService
{
    Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellation);
}

public sealed class UpdateSellService : IUpdateSellService
{
    private readonly ISellRepository _sellRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Sell> _validator;

    public UpdateSellService(ISellRepository sellRepository, IProductRepository productRepository, IValidator<Sell> validator)
    {
        _sellRepository = sellRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellation)
    {
        var validationResult = _validator.Validate(sell);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        var getSellResult = await _sellRepository.GetById(sell.Id, cancellation);
        if (getSellResult.IsFail) return new ErrorList(getSellResult.Error!);
        var oldSell = getSellResult.Success;

        var productsResult = await _productRepository.Get(cancellation);
        var updateResult = oldSell!.Update(sell, productsResult.Success!);

        if (updateResult.IsFail) return updateResult.Error!;

        var result = await _sellRepository.Update(oldSell, cancellation);
        if (result != ValidationResult.Success) return new ErrorList(result.ErrorMessage!);

        return oldSell;
    }
}
