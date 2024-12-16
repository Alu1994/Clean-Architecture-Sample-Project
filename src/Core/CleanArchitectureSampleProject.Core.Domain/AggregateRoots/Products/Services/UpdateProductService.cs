using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Services;

public interface IUpdateProductService
{
    Task<Results<Product, ErrorList>> Execute(Product productInput, CancellationToken cancellationToken);
}

public sealed class UpdateProductService : IUpdateProductService
{
    private readonly ICategoryGetOrCreateService _categoryGetOrCreateService;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _validator;

    public UpdateProductService(
        ICategoryGetOrCreateService categoryGetOrCreateService,
        IProductRepository productRepository,
        IValidator<Product> validator)
    {
        _categoryGetOrCreateService = categoryGetOrCreateService;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<Results<Product, ErrorList>> Execute(Product newProduct, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(newProduct);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        var productIdResult = await _productRepository.GetById(newProduct.Id, cancellationToken);
        if (productIdResult.IsFail) return new ErrorList(productIdResult.Error!);
        var oldProduct = productIdResult.Success!;

        var productNameResult = await ValidateIfProductNameExists(newProduct, cancellationToken);
        if (productNameResult.IsFail) return productNameResult.Error!;

        var categoryResult = await _categoryGetOrCreateService.Execute(newProduct.Category, cancellationToken);
        if (categoryResult.IsFail) return categoryResult.ToErrorList();

        Category category = categoryResult.ToSuccess();
        Product product = newProduct.Update(oldProduct, category);
        var creationResult = await _productRepository.Update(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);
        return product;
    }

    private async Task<Results<ErrorList>> ValidateIfProductNameExists(Product newProduct, CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetByName(newProduct.Name, cancellationToken);
        if (result.IsFail && result.State != ResultStates.NotFound)
            return new ErrorList(result.Error!);

        var productByName = result.Success!;
        if (result.IsSuccess && productByName.Id != newProduct.Id)
            return new ErrorList($"Product Name {newProduct.Name} already exists.");

        return ResultStates.Success;
    }
}
