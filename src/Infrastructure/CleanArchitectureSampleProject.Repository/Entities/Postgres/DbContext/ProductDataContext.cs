using CleanArchitectureSampleProject.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities;

public partial class ProductDataContext : DbContext
{
    private readonly List<IMessagingHandler> _messagings;

    public ProductDataContext(DbContextOptions<ProductDataContext> options, List<IMessagingHandler> messagings) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        _messagings = messagings;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await DispatchDomainEvents(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }

    private async Task DispatchDomainEvents(CancellationToken cancellationToken)
    {
        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
        .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            foreach (var @event in entity.DomainEvents)
            {
                var messaging = _messagings.FirstOrDefault(x => x.Event.Any(c => c == @event.GetType()));
                if (messaging is not null)
                    await messaging.SendMessage(@event.Message!, cancellationToken);
            }
            entity.ClearDomainEvents();
        }
    }    
}
