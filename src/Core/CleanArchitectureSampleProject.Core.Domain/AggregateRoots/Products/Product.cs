using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

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

    internal Product()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
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
            product.WithCategory(category);
        }

        return product;
    }

    public Product Create(Category category)
    {
        WithCategory(category);
        RegisterDomainEvent(new CreateProductEvent { ProductId = Id });
        return this;
    }

    public Product Update(Product oldProduct, Category category)
    {
        if (Id == Guid.Empty)
            Id = oldProduct.Id;

        if (string.IsNullOrWhiteSpace(Name))
            Name = oldProduct.Name;

        if (string.IsNullOrWhiteSpace(Description))
            Description = oldProduct.Description;

        if (Value is 0M)
            Value = oldProduct.Value;

        if (Quantity < 0M)
            Quantity = oldProduct.Quantity;

        WithCategory(category);
        CreationDate = oldProduct.CreationDate;

        RegisterDomainEvent(new UpdateProductEvent { ProductId = Id });
        return this;
    }

    public Product WithCategory(Category category)
    {
        Category = category;
        CategoryId = category.Id;
        return this;
    }
}
