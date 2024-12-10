using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Products;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;

public sealed class UpdateProductEvent : DomainEventBase
{
    public required Product Product { get; init; }
}

