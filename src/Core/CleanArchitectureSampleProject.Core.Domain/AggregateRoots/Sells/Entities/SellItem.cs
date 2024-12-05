using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

public sealed class SellItem
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

    public static SellItem MapToSellItem(Guid productId, Guid sellId, int quantity, decimal value = 0, Guid? id = null)
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

    public SellItem UpdateValue(decimal value)
    {
        Value = value;
        return this;
    }
}
