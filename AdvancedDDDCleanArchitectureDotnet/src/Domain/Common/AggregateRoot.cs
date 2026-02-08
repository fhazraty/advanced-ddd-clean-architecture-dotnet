namespace Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
