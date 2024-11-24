using CleanArchitectureSampleProject.Application.Outputs;
using CleanArchitectureSampleProject.CrossCuttingConcerns;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Application.Inputs;

public sealed class CategoryInput
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

    public Results<Category, BaseError> ToCategory()
    {
        if(Id is null)
            return Category.CreateNew(CategoryName);
        return Category.Create(Id, CategoryName);
    }

    internal Category ToCategory2()
    {
        return Category.MapToCategory(Id, CategoryName);
    }
}