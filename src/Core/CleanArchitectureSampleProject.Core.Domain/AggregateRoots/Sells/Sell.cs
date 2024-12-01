using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

public sealed class Sell
{
    public Guid Id { get; set; }
    public string Description { get; set; }    
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Property ====
    public ICollection<SellItem> Items { get; set; } = new List<SellItem>();

    public static Sell MapToSell(string description, decimal totalValue, ICollection<SellItem> sellItems, Guid? id = null)
    {
        var sell = new Sell
        {
            Description = description,
            TotalValue = totalValue
        };

        if (id is not null && id != Guid.Empty) sell.Id = id.Value;
        if (sellItems is not null)
        {
            sell.SetItems(sellItems);
        }

        return sell;
    }

    private void SetItems(ICollection<SellItem> sellItems)
    {
        throw new NotImplementedException();
    }
    // ==== Navigation Property ====
}
