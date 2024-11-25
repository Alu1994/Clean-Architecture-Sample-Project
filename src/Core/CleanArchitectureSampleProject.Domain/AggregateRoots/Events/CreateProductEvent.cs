using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;
using System.Text.Json;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Events;

public sealed class CreateProductEvent : DomainEventBase
{
    public CreateProductEvent(Product product)
    {
        Message = new ProductEventData(product);
    }
}

public sealed class UpdateProductEvent : DomainEventBase
{
    public UpdateProductEvent(Product product)
    {
        Message = new ProductEventData(product);
    }
}

public sealed class ProductEventData : IMessage
{
    public ProductEventData()
    {

    }

    public ProductEventData(Product product)
    {
        Body = product;
    }

    public Product? Body { get; private set; }

    public IMessage WithBody(string body)
    {
        Body = JsonSerializer.Deserialize<Product>(body)!;
        return this;
    }
}