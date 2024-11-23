using CleanArchitectureSampleProject.Domain.AggregateRoots.Products;
using CleanArchitectureSampleProject.Domain.AggregateRoots.Products.Entities;
using CleanArchitectureSampleProject.Domain.Domain.Events;
using CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureSampleProject.Infrastructure.Repository.Entities;

public class ProductDataContext : DbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category table
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreationDate);

            // One-to-Many Relationship
            entity.HasMany(u => u.Products)
                  .WithOne(o => o.Category)
                  .HasForeignKey(o => o.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete orders when a user is deleted
        });

        // Configure Product table
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreationDate);
        });
    }


    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
}