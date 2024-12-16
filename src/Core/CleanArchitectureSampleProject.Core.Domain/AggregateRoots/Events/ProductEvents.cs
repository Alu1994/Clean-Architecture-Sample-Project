using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells.Entities;
using System.Text.Json.Serialization;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;

public sealed class CreateProductEvent : DomainEventBase
{
    public required Guid ProductId { get; init; }

    [JsonConstructor]
    internal CreateProductEvent() : base() { }
}

public sealed class UpdateProductEvent : DomainEventBase
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }

    [JsonConstructor]
    internal UpdateProductEvent() : base() { }

    public static implicit operator UpdateProductEvent((Guid productId, int quantity) @event)
    {
        return new UpdateProductEvent { ProductId = @event.productId, Quantity = @event.quantity };
    }

    public static implicit operator UpdateProductEvent(SellItem sellItem)
    {
        return new UpdateProductEvent { ProductId = sellItem.ProductId, Quantity = sellItem.Quantity };
    }

    public static implicit operator UpdateProductEvent(Product product)
    {
        return new UpdateProductEvent { ProductId = product.Id, Quantity = product.Quantity };
    }
}