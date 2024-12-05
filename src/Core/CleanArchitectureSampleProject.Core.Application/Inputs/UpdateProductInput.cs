using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Validators;
using FluentValidation;

namespace CleanArchitectureSampleProject.Core.Application.Inputs;

public sealed class UpdateProductInput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public CategoryInput Category { get; set; }

    public UpdateProductInput()
    {

    }

    public void SetCategory(Category category)
    {
        if (Category is null) Category = new CategoryInput();
        Category.SetCategory(category);
    }

    public Product ToProduct(Category? category = null)
    {
        category ??= Category?.ToCategory();
        return Product.MapToProduct(Name, Description, Value, Quantity, category, Id);
    }
}

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductInput>
{
    public UpdateProductValidator()
    {
        RuleFor(product => product.Id).NotEmpty();
        RuleFor(product => product.ToProduct(null))
            .SetValidator(new ProductValidator());
    }
}
