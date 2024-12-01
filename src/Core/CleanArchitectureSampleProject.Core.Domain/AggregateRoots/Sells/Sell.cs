using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

public sealed class Sell
{
    public Guid Id { get; set; }
    public string Description { get; set; }    
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Property ====
    public ICollection<SellItem> Items { get; set; } = new List<SellItem>();
    // ==== Navigation Property ====
}
