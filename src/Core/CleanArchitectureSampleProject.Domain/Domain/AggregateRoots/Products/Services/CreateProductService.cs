using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;

public interface ICreateProductService
{
    Task<Results<Product, BaseError>> Execute(Product productInput, Category categoryInput, CancellationToken cancellationToken);
}

public sealed class CreateProductService : ICreateProductService
{
    private readonly ICategoryGetOrCreateService _categoryGetOrCreateService;
    private readonly IProductRepository _productRepository;

    public CreateProductService(ICategoryGetOrCreateService categoryGetOrCreateService, IProductRepository productRepository)
    {
        _categoryGetOrCreateService = categoryGetOrCreateService;
        _productRepository = productRepository;
    }

    public async Task<Results<Product, BaseError>> Execute(Product productInput, Category categoryInput, CancellationToken cancellationToken)
    {
        // Verify is category exists.
        //// If error while creating/getting category, return error.
        var categoryResult = await _categoryGetOrCreateService.Execute(categoryInput, cancellationToken);
        if (categoryResult.IsFail) return (categoryResult.State, categoryResult.Error)!;

        // If category was found or created, tries to create product.
        //// If product already exists, return error.
        Category category = categoryResult.ToSuccess();
        productInput.SetCategory(category);
        var productResult = productInput.Validate();
        if (productResult.IsFail) return productResult;

        Product product = productResult.ToSuccess();
        var getProductByIdResult = await _productRepository.GetByName(product.Name, cancellationToken);
        if (getProductByIdResult.State is ResultStates.Error) return getProductByIdResult.Error!;
        if (getProductByIdResult.IsSuccess) return new BaseError($"Product '{product.Id}' - '{product.Name}' already exists!");

        // If product does not exists try to create it.
        //// If error while creating product, return error.
        product.Create();
        var creationResult = await _productRepository.Insert(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new BaseError(creationResult.ErrorMessage!);

        // If SUCCESS return product.
        return product;
    }
}
