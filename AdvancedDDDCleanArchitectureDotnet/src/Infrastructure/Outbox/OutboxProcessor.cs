using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxProcessor(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var batch = await db.Outbox
                .FromSqlRaw(@"
                    SELECT TOP (20) *
                    FROM dbo.Outbox WITH (READPAST, ROWLOCK)
                    WHERE ProcessedOnUtc IS NULL
                    ORDER BY OccurredOnUtc")
                .AsNoTracking()
                .ToListAsync(stoppingToken);

            foreach (var msg in batch)
            {
                var processedOn = DateTime.UtcNow;
                string? error = null;

                try
                {
                    // Publish...
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                // آپدیت شرطی: اگر قبلاً پردازش شده باشد rowsAffected = 0 و ما ادامه می‌دهیم
                _ = await db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE dbo.Outbox
                    SET ProcessedOnUtc = {processedOn},
                        Error = {error}
                    WHERE Id = {msg.Id}
                      AND ProcessedOnUtc IS NULL", stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
