using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Products;

public sealed class CreateProductInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public UpdateCategoryInput Category { get; set; }

    public CreateProductInput()
    {

    }

    public CreateProductInput(UpdateProductInput input)
    {
        Name = input.Name;
        Description = input.Description;
        Value = input.Value;
        Quantity = input.Quantity;
        Category = input.Category;
    }

    public void SetCategory(Category category)
    {
        Category ??= new UpdateCategoryInput();
        Category.SetCategory(category);
    }

    public Product ToProduct(Category? category = null)
    {
        category ??= Category?.ToCategory();
        return Product.MapToProduct(Name, Description, Value, Quantity, category);
    }
}

public sealed class CreateProductInputValidator : AbstractValidator<CreateProductInput>
{
    public CreateProductInputValidator()
    {
        RuleFor(product => product.ToProduct(null))
            .SetValidator(new ProductValidator());
    }
}