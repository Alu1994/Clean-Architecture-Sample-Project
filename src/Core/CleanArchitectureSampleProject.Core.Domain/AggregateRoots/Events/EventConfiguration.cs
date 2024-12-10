using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;

public abstract class DomainEventBase
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEventBase> DomainEvents { get; }
}

public abstract class HasDomainEventsBase
{
    private readonly List<DomainEventBase> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}