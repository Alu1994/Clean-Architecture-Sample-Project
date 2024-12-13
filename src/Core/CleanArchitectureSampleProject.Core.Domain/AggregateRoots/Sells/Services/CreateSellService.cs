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

        var products = await _productRepository.Get(cancellation);
        var createResult = sell.Create(products.Success!);

        if (createResult.IsFail) return createResult.Error!;

        var result = await _sellRepository.Insert(sell, cancellation);
        if (result != ValidationResult.Success) return new ErrorList(result.ErrorMessage!);

        return sell;
    }
}
