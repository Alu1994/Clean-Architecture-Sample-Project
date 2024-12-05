using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Core.Application.Outputs;

public sealed class GetSellOutput
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }
    public FrozenSet<GetSellItemOutput> Items { get; set; }

    public GetSellOutput()
    {

    }


    public static implicit operator GetSellOutput(Sell sell)
    {
        var sellOutput = new GetSellOutput
        {
            Id = sell.Id,
            Description = sell.Description,
            CreationDate = sell.CreationDate,
            TotalValue = sell.TotalValue,
        };

        if(sell.Items is { Count: 0 })
        {
            sellOutput.Items = FrozenSet<GetSellItemOutput>.Empty;
            return sellOutput;
        }

        var items = new Collection<GetSellItemOutput>();
        foreach (var item in sell.Items)
        {
            items.Add(item);
        }
        sellOutput.Items = items.ToFrozenSet();
        return sellOutput;
    }
}


public sealed class GetSellItemOutput
{
    public Guid Id { get; set; }
    public Guid SellId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Value { get; set; }
    public int Quantity { get; set; }
    public DateTime CreationDate { get; set; }


    public static implicit operator GetSellItemOutput(SellItem sellItem)
    {
        return new GetSellItemOutput
        {
            Id = sellItem.Id,
            SellId = sellItem.SellId,
            ProductId = sellItem.ProductId,
            Value = sellItem.Value,
            Quantity = sellItem.Quantity,
            CreationDate = sellItem.CreationDate,
        };
    }
}