using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Services;

public interface ICreateSellService
{
    Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellation);
}

public sealed class CreateSellService : ICreateSellService
{
    private readonly ISellRepository _sellRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Sell> _validator;

    public CreateSellService(ISellRepository sellRepository, IProductRepository productRepository, IValidator<Sell> validator)
    {
        _sellRepository = sellRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<Results<Sell, ErrorList>> Execute(Sell sell, CancellationToken cancellation)
    {
        var validationResult = _validator.Validate(sell);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        await UpdateValues(sell, cancellation);
        sell.Create();

        var creationResult = await _sellRepository.Insert(sell, cancellation);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        return sell;
    }

    private async Task UpdateValues(Sell sell, CancellationToken cancellation)
    {
        var products = await _productRepository.Get(cancellation);
        var items = products.Success!.Where(x => sell.Items.Select(x2 => x2.ProductId).Contains(x.Id));
        var totalValue = 0M;
        foreach (var i in items)
        {
            var sellItem = sell.Items.FirstOrDefault(x => x.ProductId == i.Id);
            if (sellItem is null) continue;
            sellItem.UpdateValue(i.Value);
            totalValue += i.Value * sellItem.Quantity;
        }

        sell.UpdateTotalValue(totalValue);
    }
}
