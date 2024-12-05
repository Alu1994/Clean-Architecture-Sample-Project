using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class CreateSellItemInput
{
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }

    public CreateSellItemInput()
    {
        
    }

    public CreateSellItemInput(Guid sellId, CreateSellItemInput item)
    {
        SellId = sellId;
        ProductId = item.ProductId;
        Value = item.Value;
        Quantity = item.Quantity;
    }

    public static implicit operator SellItem(CreateSellItemInput sellItemInput)
    {
        return SellItem.MapToSellItem(sellItemInput.ProductId, sellItemInput.SellId, sellItemInput.Quantity, sellItemInput.Value);
    }
}
