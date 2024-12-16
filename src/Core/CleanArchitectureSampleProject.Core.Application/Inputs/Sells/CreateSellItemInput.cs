using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class CreateSellItemInput
{
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }

    public SellItem ToSellItem(Guid sellId)
    {
        var sellItem = SellItem.ToSellItem(ProductId, sellId, Quantity, Value);
        return sellItem;
    }
}
