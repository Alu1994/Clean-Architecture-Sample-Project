using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;
using CleanArchitectureSampleProject.Infrastructure.Repository.Entities.Postgres.AggregateRoots.Products;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities;

public partial class ProductDataContext : DbContext
{
    private readonly IBus _bus;

    public ProductDataContext(DbContextOptions<ProductDataContext> options, IBus bus) : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await DispatchDomainEvents(cancellationToken);
        return result;
    }

    public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
        await DispatchDomainEvents(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());

        modelBuilder.ApplyConfiguration(new SellConfiguration());
        modelBuilder.ApplyConfiguration(new SellItemConfiguration());
    }

    private async Task DispatchDomainEvents(CancellationToken cancellationToken)
    {
        try
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
                    await _bus.Publish(@event, @event.GetType(), cancellationToken);
                }
                entity.ClearDomainEvents();
            }
        }
        catch (Exception ex)
        {
            string message = ex.Message;
        }
    }
}
