﻿using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.Domain.AggregateRoots.Products.Services;

public interface IUpdateProductService
{
    Task<Results<Product, BaseError>> Execute(Product productInput, Category categoryInput, CancellationToken cancellationToken);
}

public sealed class UpdateProductService : IUpdateProductService
{
    private readonly ICategoryGetOrCreateService _categoryGetOrCreateService;
    private readonly IProductRepository _productRepository;

    public UpdateProductService(ICategoryGetOrCreateService categoryGetOrCreateService, IProductRepository productRepository)
    {
        _categoryGetOrCreateService = categoryGetOrCreateService;
        _productRepository = productRepository;
    }

    public async Task<Results<Product, BaseError>> Execute(Product productInput, Category categoryInput, CancellationToken cancellationToken)
    {
        // Verify is category exists.
        //// If error while creating/getting category, return error.
        var categoryResult = await _categoryGetOrCreateService.Execute(categoryInput, cancellationToken);
        if (categoryResult.IsFail) return categoryResult.ToError();

        // If category was found or created, tries to update product.
        //// If product does not exists, return error.
        Category category = categoryResult.ToSuccess();
        productInput.SetCategory(category);
        var productResult = productInput.Validate();
        if (productResult.IsFail) return productResult;

        Product product = productResult.ToSuccess().Update(productInput.Id);
        var getProductByIdResult = await _productRepository.GetById(product.Id, cancellationToken);
        if (getProductByIdResult.IsFail) return getProductByIdResult;
        if (getProductByIdResult.ToSuccess() is null) return new BaseError($"Product '{product.Id}' - '{product.Name}' does not exists!");

        // If product exists try to update it.
        //// If error while updating product, return error.
        var creationResult = await _productRepository.Update(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new BaseError(creationResult.ErrorMessage!);

        // If SUCCESS return product.
        product.SetCategory(category);
        return product;
    }
}
