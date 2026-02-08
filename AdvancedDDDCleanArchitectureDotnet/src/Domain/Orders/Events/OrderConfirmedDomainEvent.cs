using Domain.Common;

namespace Domain.Orders.Events
{
    public sealed record OrderConfirmedDomainEvent(Guid OrderId) : DomainEvent;
}
