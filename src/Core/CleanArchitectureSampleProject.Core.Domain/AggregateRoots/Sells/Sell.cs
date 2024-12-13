using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;

public sealed class Sell : HasDomainEventsBase
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime CreationDate { get; set; }

    // ==== Navigation Property ====
    public ICollection<SellItem> Items { get; set; } = new List<SellItem>();

    public static Sell ToSell(string description, Guid? id = null)
    {
        var sell = new Sell
        {
            Description = description
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

    public Results<Sell, ErrorList> Create(FrozenSet<Product> products)
    {
        Collection<ErrorItem> errors = [];
        foreach (var item in Items)
        {
            var productItem = products.FirstOrDefault(x => item.ProductId == x.Id);
            if (productItem is null)
            {
                errors.Add(new ErrorItem($"ProductId: '{item.ProductId}' not found for ItemId: '{item.Id}'"));
                continue;
            }
            item.Create(productItem);

            RegisterDomainEvent((UpdateProductEvent)item);
            TotalValue += item.TotalValue();
        }

        if (errors.Count is not 0)
        {
            ClearDomainEvents();
            return new ErrorList(errors);
        }

        return this;
    }

    internal Results<ErrorList> Update(Sell sell, FrozenSet<Product> products)
    {
        Collection<ErrorItem> errors = [];
        Description = sell.Description;

        foreach (var newItem in sell.Items)
        {
            var productItem = products!.FirstOrDefault(product => product.Id == newItem.ProductId);
            if (productItem is null)
            {
                errors.Add(new ErrorItem($"ProductId: '{newItem.ProductId}' not found for ItemId: '{newItem.Id}'"));
                continue;
            }
            var oldSellItem = Items.FirstOrDefault(x => x.Id == newItem.Id);
            if (newItem.Id != Guid.Empty && oldSellItem is null)
            {
                errors.Add(new ErrorItem($"ItemId: '{newItem.Id}' not found for Sell: '{Id}'."));
                continue;
            }

            if (newItem.Id == Guid.Empty)
            {
                Items.Add(newItem);

                RegisterDomainEvent((UpdateProductEvent)newItem);
                TotalValue += newItem.Update(productItem);
                continue;
            }

            RegisterDomainEvent((UpdateProductEvent)(newItem.ProductId, newItem.Quantity - oldSellItem!.Quantity));
            TotalValue += oldSellItem!.Update(newItem, productItem);
        }

        if (errors.Count > 0)
        {
            ClearDomainEvents();
            return new ErrorList(errors);
        }

        return ResultStates.Success;
    }
    // ==== Navigation Property ====
}
