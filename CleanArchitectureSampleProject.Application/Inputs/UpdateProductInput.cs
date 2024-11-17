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

    public Validation<Error, Product> ToProduct()
    {
        var category = Category.ToCategory();
        return category.Match(cat =>
        {
            return Product.CreateExistent(Id, Name, Description, Value, Quantity, cat);
        }, err => err);
    }
}
