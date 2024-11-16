using TrainingTDDWithCleanArch.Domain.AggregateRoots.Products.Entities;

namespace TrainingTDDWithCleanArch.Domain.AggregateRoots.Products;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public Category Category { get; set; }

    public Product()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    private Product(Category category) : this()
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
    }

    public static Validation<Error, Product> CreateExistent(Guid id, string name, string description, decimal? value, int? quantity, Category category)
    {
        return new Product(category)
        {
            Name = name,
            Description = description,
            Value = value.Value,
            Quantity = quantity.Value
        }.WithId(id);
    }

    public static Validation<Error, Product> CreateNew(string name, string description, decimal? value, int? quantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.New($"{nameof(Name)} must not be null.");

        if (string.IsNullOrWhiteSpace(description))
            return Error.New($"{nameof(Description)} must not be null.");

        if (value is null)
            return Error.New($"{nameof(Value)} must not be null.");

        if (quantity is null)
            return Error.New($"{nameof(Quantity)} must not be null.");

        if (category is null)
            return Error.New($"{nameof(Category)} must not be null.");

        return new Product(category)
        {
            Name = name,
            Description = description,
            Value = value.Value,
            Quantity = quantity.Value
        };
    }

    public ValidationResult ChangeCategory(Category category)
    {
        if (category is null)
        {
            return new ValidationResult($"{nameof(Category)} must not be null.");
        }
        Category = category;
        return ValidationResult.Success!;
    }

    public ValidationResult UpdateValue(decimal? value)
    {
        if (value is null)
        {
            return new ValidationResult($"{nameof(Value)} must not be null.");
        }
        Value = value.Value;
        return ValidationResult.Success!;
    }

    public ValidationResult AddQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return new ValidationResult($"{nameof(Quantity)} must not be null.");
        }
        Quantity += quantity.Value;
        return ValidationResult.Success!;
    }

    public ValidationResult SubtractQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return new ValidationResult($"{nameof(Quantity)} must not be null.");
        }
        Quantity -= quantity.Value;
        return ValidationResult.Success!;
    }

    public void SetCategory(Category category)
    {
        Category = category;
    }

    public Product WithCategory(Category category)
    {
        SetCategory(category);
        return this;
    }

    private Product WithId(Guid id)
    {
        Id = id;
        return this;
    }
}
