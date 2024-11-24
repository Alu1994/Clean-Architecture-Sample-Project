using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Application.Inputs;

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

    public Product ToProduct(Category category)
    {
        return Product.MapToProduct(Name, Description, Value, Quantity, category, Id);
    }
}
