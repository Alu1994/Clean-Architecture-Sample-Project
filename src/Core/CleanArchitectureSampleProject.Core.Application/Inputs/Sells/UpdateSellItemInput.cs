using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Application.Inputs.Sells;

public sealed class UpdateSellItemInput
{
    public Guid? Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public UpdateSellItemInput()
    {

    }

    public UpdateSellItemInput(Guid sellId, UpdateSellItemInput item)
    {
        SellId = sellId;
        Id = item.Id;
        ProductId = item.ProductId;
        Quantity = item.Quantity;
    }

    public static implicit operator SellItem(UpdateSellItemInput sellItemInput)
    {
        return SellItem.MapToSellItem(sellItemInput.ProductId, sellItemInput.SellId, sellItemInput.Quantity, sellItemInput.Id);
    }
}