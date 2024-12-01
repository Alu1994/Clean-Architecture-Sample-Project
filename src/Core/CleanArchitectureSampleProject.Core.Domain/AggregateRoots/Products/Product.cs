using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

public sealed class Product : HasDomainEventsBase
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public Guid CategoryId { get; set; }

    // ==== Navigation Properties ====
    public Category Category { get; set; }
    public ICollection<SellItem> Items { get; set; } = new List<SellItem>();
    // ==== Navigation Properties ====

    public Product()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    private Product(Category category) : this()
    {
        ArgumentNullException.ThrowIfNull(category);
        SetCategory(category);
    }

    private Product WithCreationDate(DateTime? creationDate)
    {
        if (creationDate is not null)
            CreationDate = creationDate.Value;
        return this;
    }

    public static Product MapToProduct(string name, string description, decimal? value, int? quantity, Category? category, Guid? id = null)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Value = value.Value,
            Quantity = quantity.Value
        };

        if (id is not null && id != Guid.Empty) product.Id = id.Value;
        if (category is not null)
        {
            product.SetCategory(category);
        }

        return product;
    }

    public void SetCategory(Category category)
    {
        Category = category;
        CategoryId = category.Id;
    }

    public Product Create()
    {
        RegisterDomainEvent(new CreateProductEvent(this));
        return this;
    }

    public Product Update(Guid id)
    {
        Id = id;
        RegisterDomainEvent(new UpdateProductEvent(this));
        return this;
    }

    public Product WithCategory(Category category)
    {
        SetCategory(category);
        return this;
    }

    private Product WithId(Guid id)
    {
        Id = id;

        RegisterDomainEvent(new UpdateProductEvent(this));
        return this;
    }
}
