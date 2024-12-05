using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;
using System.Threading;

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


        oldSell!.Update(sell);
        await UpdateValues(oldSell, cancellation);
        oldSell.Create();


        var creationResult = await _sellRepository.Update(oldSell, cancellation);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        return oldSell;
    }

    private async Task UpdateValues(Sell sell, CancellationToken cancellation)
    {
        var products = await _productRepository.Get(cancellation);
        var productItems = products.Success!
            .Where(product => 
                sell.Items
                    .Select(sellItem => sellItem.ProductId)
                    .Contains(product.Id));

        var totalValue = 0M;

        foreach (var item in sell.Items)
        {
            var productItem = productItems.First(x => item.ProductId == x.Id);
            if (productItem is null) continue;
            if(item is { Value: <= 0 })
                item.UpdateValue(productItem.Value);
            totalValue += item.Value * item.Quantity;
        }

        sell.UpdateTotalValue(totalValue);

        //foreach (var i in productItems)
        //{
        //    var sellItem = sell.Items.FirstOrDefault(x => x.ProductId == i.Id);
        //    if (sellItem is null) continue;
        //    sellItem.UpdateValue(i.Value);
        //    totalValue += i.Value * sellItem.Quantity;
        //}
    }
}
