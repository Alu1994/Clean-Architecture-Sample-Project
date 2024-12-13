using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

public sealed class SellItem : HasDomainEventsBase
{
    public Guid Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Properties ====
    public Sell Sell { get; set; }
    public Product Product { get; set; }
    // ==== Navigation Properties ====

    public static SellItem ToSellItem(Guid productId, Guid sellId, int quantity, decimal value = 0, Guid? id = null)
    {
        var sellItem = new SellItem
        {
            ProductId = productId,
            SellId = sellId,
            Quantity = quantity,
            Value = value,
        };
        if (id is not null && id != Guid.Empty) sellItem.Id = id.Value;
        return sellItem;
    }

    public SellItem Create(Product product)
    {
        return UpdateValueBy(product);
    }

    internal decimal Update(Product? product)
    {
        if (product is null) return 0M;
        UpdateValueBy(product);
        return Value * Quantity;
    }

    internal decimal Update(SellItem newItem, Product? product)
    {
        decimal oldTotalValue = Quantity * Value;
        Quantity = newItem.Quantity;
        Value = newItem.Value;
        decimal result = Update(product) - oldTotalValue;
        return result;
    }

    internal decimal TotalValue() => Value * Quantity;

    private SellItem UpdateValueBy(Product product)
    {
        if (Value <= 0M)
            Value = product.Value;
        return this;
    }
}
