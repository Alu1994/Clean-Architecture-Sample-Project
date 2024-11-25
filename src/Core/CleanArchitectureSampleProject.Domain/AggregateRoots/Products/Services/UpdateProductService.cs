using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Services;

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

    public async Task<Results<Product, ErrorList>> Execute(Product productInput, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(productInput);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        var getProductByIdResult = await _productRepository.GetById(productInput.Id, cancellationToken);
        if (getProductByIdResult.IsFail) return new ErrorList(getProductByIdResult.Error);

        var categoryResult = await _categoryGetOrCreateService.Execute(productInput.Category, cancellationToken);
        if (categoryResult.IsFail) return categoryResult.ToErrorList();

        Category category = categoryResult.ToSuccess();
        productInput.SetCategory(category);

        Product product = productInput.Update(productInput.Id);
        var creationResult = await _productRepository.Update(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);

        product.SetCategory(category);
        return product;
    }
}
