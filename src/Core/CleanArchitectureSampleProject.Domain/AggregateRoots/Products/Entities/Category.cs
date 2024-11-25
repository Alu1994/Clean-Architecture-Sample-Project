using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;

public sealed class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>();

    internal Category()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    public static Results<Category, BaseError> CreateNew(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return new BaseError($"{nameof(Category)}.{nameof(Name)} must not be null.");

        return new Category
        {
            Name = categoryName
        };
    }

    public static Results<Category, BaseError> Create(Guid? id, string categoryName, DateTime? creationDate = null)
    {
        if (id is null)
            return new BaseError($"{nameof(Category)}.{nameof(Id)} must not be null.");

        if (string.IsNullOrWhiteSpace(categoryName))
            return new BaseError($"{nameof(Category)}.{nameof(Name)} must not be null.");

        return new Category
        {
            Id = id.Value,
            Name = categoryName,
            CreationDate = creationDate ?? DateTime.MinValue
        };
    }

    public Category Update(Category category)
    {
        Name = category.Name;
        return this;
    }

    internal Results<Category, BaseError> Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return new BaseError($"{nameof(Category)}.{nameof(Name)} must not be null.");
        return this;
    }

    internal Results<Category, BaseError> ValidateGetOrCreate()
    {
        if (string.IsNullOrWhiteSpace(Name) && Id == Guid.Empty)
            return new BaseError($"{nameof(Category)}.{nameof(Id)} or {nameof(Category)}.{nameof(Name)} must not be null.");
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

    internal void Create()
    {
        Id = Guid.NewGuid();
    }
}