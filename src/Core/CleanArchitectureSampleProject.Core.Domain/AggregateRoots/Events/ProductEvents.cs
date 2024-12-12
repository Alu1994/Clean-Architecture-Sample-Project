namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;

public sealed class CreateProductEvent : DomainEventBase
{
    public required Guid ProductId { get; init; }
}

public sealed class UpdateProductEvent : DomainEventBase
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}