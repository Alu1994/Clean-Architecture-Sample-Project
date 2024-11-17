using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

namespace CleanArchitectureSampleProject.Application.Outputs;

public sealed class CategoryOutput
{
    public Guid? Id { get; set; }
    public string? CategoryName { get; set; }
    public DateTime CreationDate { get; set; }

    public CategoryOutput()
    {

    }

    public static implicit operator CategoryOutput(Category category)
    {
        return new CategoryOutput
        {
            Id = category.Id,
            CategoryName = category.Name,
            CreationDate = category.CreationDate
        };
    }
}