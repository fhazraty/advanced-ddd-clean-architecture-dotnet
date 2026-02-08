using Domain.Common;

namespace Domain.Orders.Events
{
    public sealed record OrderPaidDomainEvent(Guid OrderId, string PaymentRef) : DomainEvent;
}
