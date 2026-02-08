using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public abstract record DomainEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}
