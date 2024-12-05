using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
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

        var getProductByIdResult = await _productRepository.GetById(newProduct.Id, cancellationToken);
        if (getProductByIdResult.IsFail) return new ErrorList(getProductByIdResult.Error);
        var oldProduct = getProductByIdResult.Success!;

        var categoryResult = await _categoryGetOrCreateService.Execute(newProduct.Category, cancellationToken);
        if (categoryResult.IsFail) return categoryResult.ToErrorList();

        Category category = categoryResult.ToSuccess();
        newProduct.SetCategory(category);

        Product product = oldProduct.Update(newProduct);
        var creationResult = await _productRepository.Update(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        product.SetCategory(category);
        return product;
    }
}
