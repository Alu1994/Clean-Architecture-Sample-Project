using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using System.Collections.Frozen;

namespace CleanArchitectureSampleProject.Core.Application.Outputs.Sells;

public sealed class CreateSellOutput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }
    public FrozenSet<CreateSellItemOutput> Items { get; set; }

    public CreateSellOutput()
    {

    }


    public static implicit operator CreateSellOutput(Sell sell)
    {
        return new CreateSellOutput
        {
            Id = sell.Id,
            Description = sell.Description,
            TotalValue = sell.TotalValue,
            CreationDate = sell.CreationDate,
            Items = sell.Items.Select<SellItem, CreateSellItemOutput>(x => x).ToFrozenSet(),
        };
    }
}

public sealed class CreateSellItemOutput
{
    public Guid Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public DateTime CreationDate { get; set; }

    public static implicit operator CreateSellItemOutput(SellItem sellItem)
    {
        return new CreateSellItemOutput
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
