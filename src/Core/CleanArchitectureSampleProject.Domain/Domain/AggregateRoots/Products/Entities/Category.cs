namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;

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

    public static Validation<Error, Category> CreateNew(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return Error.New($"{nameof(Category)}.{nameof(Name)} must not be null.");
        
        return new Category
        {
            Name = categoryName
        };
    }

    public static Validation<Error, Category> Create(Guid? id, string categoryName, DateTime? creationDate = null)
    {
        if (id is null)
            return Error.New($"{nameof(Category)}.{nameof(Id)} must not be null.");

        if (string.IsNullOrWhiteSpace(categoryName))
            return Error.New($"{nameof(Category)}.{nameof(Name)} must not be null.");

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
}