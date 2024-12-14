using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Products;

public class CreateCategoryInput
{
    public string? CategoryName { get; set; }

    public CreateCategoryInput()
    {

    }

    public void SetCategory(Category category)
    {
        CategoryName = category.Name;
    }

    public Category ToCategory()
    {
        return Category.MapToCategory(Guid.Empty, CategoryName);
    }
}