﻿using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Repositories;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Services;

public interface ICreateProductService
{
    Task<Results<Product, IError>> Execute(Product productInput, CancellationToken cancellationToken);
}

public sealed class CreateProductService : ICreateProductService
{
    private readonly ICategoryGetOrCreateService _categoryGetOrCreateService;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _validator;

    public CreateProductService(ICategoryGetOrCreateService categoryGetOrCreateService, IProductRepository productRepository, IValidator<Product> validator)
    {
        _categoryGetOrCreateService = categoryGetOrCreateService;
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<Results<Product, IError>> Execute(Product product, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(product);
        if (validationResult.IsValid is false) return new ErrorList(validationResult);

        var getProductByIdResult = await _productRepository.GetByName(product.Name, cancellationToken);
        if (getProductByIdResult.State is ResultStates.Error) return getProductByIdResult.Error!;
        if (getProductByIdResult.IsSuccess) return new ErrorList($"Product '{product.Id}' - '{product.Name}' already exists!");

        var categoryResult = await _categoryGetOrCreateService.Execute(product.Category, cancellationToken);
        if (categoryResult.IsFail) return (categoryResult.State, categoryResult.Error)!;

        Category category = categoryResult.ToSuccess();
        product.SetCategory(category);

        product.Create();
        var creationResult = await _productRepository.Insert(product, cancellationToken);
        if (creationResult != ValidationResult.Success) return new ErrorList(creationResult.ErrorMessage!);
                
        product.SetCategory(category);
        return product;
    }
}
