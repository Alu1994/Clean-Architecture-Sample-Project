using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

public sealed class Sell : HasDomainEventsBase
{
    public Guid Id { get; set; }
    public string Description { get; set; }    
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Property ====
    public ICollection<SellItem> Items { get; set; } = new List<SellItem>();

    public static Sell MapToSell(string description, decimal totalValue, Guid? id = null)
    {
        var sell = new Sell
        {
            Description = description,
            TotalValue = totalValue
        };

        if (id is not null && id != Guid.Empty)
        {
            sell.Id = id.Value;
            return sell;
        }
        sell.Id = Guid.NewGuid();
        return sell;
    }

    public void SetItems(ICollection<SellItem> sellItems)
    {
        Items = sellItems;
    }

    public Sell Create()
    {
        // Send Update Stock Event
        RegisterDomainEvent(new CreateSellEvent(this));
        return this;
    }

    internal Sell UpdateTotalValue(decimal v)
    {
        throw new NotImplementedException();
    }
    // ==== Navigation Property ====
}
