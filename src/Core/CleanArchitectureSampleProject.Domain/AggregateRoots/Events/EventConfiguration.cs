using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitectureSampleProject.Domain.AggregateRoots.Events;

public abstract class HasDomainEventsBase
{
    private readonly List<DomainEventBase> _domainEvents = new();
    [NotMapped]
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class DomainEventBase
{
    public IMessage? Message { get; set; }
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEventBase> DomainEvents { get; }
}