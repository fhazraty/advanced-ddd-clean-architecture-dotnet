using Domain.Common;
using Domain.Orders;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

//Add-Migration Init -Project Infrastructure -StartupProject Api
//Update-Database -Project Infrastructure -StartupProject Api


namespace Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 1) Collect domain events
            var aggregates = ChangeTracker.Entries()
                .Where(e => e.Entity is AggregateRoot)
                .Select(e => (AggregateRoot)e.Entity)
                .ToList();

            var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

            // 2) Convert domain events to outbox messages
            foreach (var ev in domainEvents)
            {
                Outbox.Add(new OutboxMessage
                {
                    OccurredOnUtc = ev.OccurredOnUtc,
                    Type = ev.GetType().FullName!,
                    Content = JsonSerializer.Serialize(ev, ev.GetType())
                });
            }

            var result = await base.SaveChangesAsync(ct);

            // 3) Clear after success
            aggregates.ForEach(a => a.ClearDomainEvents());

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
