using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using System.Linq;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

public sealed class UpdateSellOutput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }
    public List<UpdateSellItemOutput> Items { get; set; }

    public UpdateSellOutput()
    {

    }


    public static implicit operator UpdateSellOutput(Sell sell)
    {
        return new UpdateSellOutput
        {
            Id = sell.Id,
            Description = sell.Description,
            TotalValue = sell.TotalValue,
            CreationDate = sell.CreationDate,
            Items = sell.Items.Select<SellItem, UpdateSellItemOutput>(x => x).ToList(),
        };
    }
}

public sealed class UpdateSellItemOutput
{
    public Guid Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public DateTime CreationDate { get; set; }

    public static implicit operator UpdateSellItemOutput(SellItem sellItem)
    {
        return new UpdateSellItemOutput
        {
            Id = sellItem.Id,
            SellId = sellItem.SellId,
            ProductId = sellItem.ProductId,
            Quantity = sellItem.Quantity,
            Value = sellItem.Value,
            CreationDate = sellItem.CreationDate
        };
    }
}
