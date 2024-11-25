using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Application.Inputs;

public class CategoryInput
{
    public Guid? Id { get; set; }
    public string? CategoryName { get; set; }

    public CategoryInput()
    {

    }

    public void SetCategory(Category category)
    {
        Id = category.Id;
        CategoryName = category.Name;
    }

    public Category ToCategory()
    {
        return Category.MapToCategory(Id, CategoryName);
    }
}

public sealed class UpdateCategoryInput : CategoryInput;