using Domain.Common;

namespace Domain.Orders.Events
{
    public sealed record OrderCreatedDomainEvent(Guid OrderId, string Email) : DomainEvent;
}
