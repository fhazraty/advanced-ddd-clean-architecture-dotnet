using Domain.Common;
using Domain.Orders.Entities;
using Domain.Orders.Events;

namespace Domain.Orders
{
    public enum OrderStatus { Draft = 0, Confirmed = 1, Paid = 2, Cancelled = 3 }

    public sealed class Order : AggregateRoot
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Email CustomerEmail { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;

        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order() { } // EF

        private Order(Email email)
        {
            CustomerEmail = email;
            Raise(new OrderCreatedDomainEvent(Id, email.Value));
        }

        public static Result<Order> Create(Email email)
            => Result<Order>.Success(new Order(email));

        public Result AddItem(Guid productId, Money unitPrice, int qty)
        {
            if (Status != OrderStatus.Draft)
                return Result.Failure(new Error("order.not_draft", "فقط در وضعیت Draft می‌توان آیتم اضافه کرد."));

            var itemResult = OrderItem.Create(productId, unitPrice, qty);
            if (!itemResult.IsSuccess) return Result.Failure(itemResult.Error!);

            _items.Add(itemResult.Value!);
            return Result.Success();
        }

        public Result Confirm()
        {
            if (Status != OrderStatus.Draft)
                return Result.Failure(new Error("order.state", "سفارش قابل تأیید نیست."));

            if (_items.Count == 0)
                return Result.Failure(new Error("order.empty", "سفارش بدون آیتم قابل تأیید نیست."));

            Status = OrderStatus.Confirmed;
            Raise(new OrderConfirmedDomainEvent(Id));
            return Result.Success();
        }

        public Result MarkPaid(string paymentRef)
        {
            if (Status != OrderStatus.Confirmed)
                return Result.Failure(new Error("order.not_confirmed", "فقط سفارش Confirmed قابل پرداخت است."));

            if (string.IsNullOrWhiteSpace(paymentRef))
                return Result.Failure(new Error("payment.ref", "شماره مرجع پرداخت الزامی است."));

            Status = OrderStatus.Paid;
            Raise(new OrderPaidDomainEvent(Id, paymentRef));
            return Result.Success();
        }

        public Money Total()
        {
            // فرض: همه آیتم‌ها یک currency دارند
            if (_items.Count == 0) return Money.Create(0, "IRR").Value!;
            var currency = _items[0].UnitPrice.Currency;

            var total = Money.Create(0, currency).Value!;
            foreach (var item in _items)
                total += item.LineTotal();

            return total;
        }
    }
}
