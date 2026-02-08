namespace Infrastructure.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; set; }
        public string Type { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }
    }
}
