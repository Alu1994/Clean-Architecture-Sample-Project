namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;

public sealed class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Property ====
    public ICollection<Product> Products { get; set; } = [];
    // ==== Navigation Property ====

    internal Category()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    internal void Create()
    {
        Id = Guid.NewGuid();
    }

    internal Category Update(Category category)
    {
        Name = category.Name;
        return this;
    }

    public static Category MapToCategory(Guid? id, string? categoryName)
    {
        if (id is null || id == Guid.Empty)
        {
            var cat = new Category { Name = categoryName };
            cat.Id = Guid.Empty;
            return cat;
        }

        return new Category
        {
            Id = id.Value,
            Name = categoryName
        };
    }
}