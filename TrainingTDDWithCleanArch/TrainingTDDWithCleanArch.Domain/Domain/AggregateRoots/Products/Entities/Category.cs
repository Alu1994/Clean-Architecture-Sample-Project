namespace TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

public sealed class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }

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

    public static Validation<Error, Category> Create(Guid? id, string categoryName)
    {
        if (id is null)
            return Error.New($"{nameof(Category)}.{nameof(Id)} must not be null.");

        if (string.IsNullOrWhiteSpace(categoryName))
            return Error.New($"{nameof(Category)}.{nameof(Name)} must not be null.");

        return new Category
        {
            Id = id.Value,
            Name = categoryName
        };
    }
}