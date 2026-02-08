using Domain.Common;

namespace Domain.Orders.Entities
{
    public sealed class OrderItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; }
        public Money UnitPrice { get; private set; } = default!;
        public int Quantity { get; private set; }

        private OrderItem() { } // EF

        private OrderItem(Guid productId, Money unitPrice, int quantity)
        {
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public static Result<OrderItem> Create(Guid productId, Money unitPrice, int quantity)
        {
            if (productId == Guid.Empty)
                return Result<OrderItem>.Failure(new Error("item.product", "محصول نامعتبر است."));

            if (quantity <= 0)
                return Result<OrderItem>.Failure(new Error("item.qty", "تعداد باید بزرگتر از صفر باشد."));

            return Result<OrderItem>.Success(new OrderItem(productId, unitPrice, quantity));
        }

        public Money LineTotal() => UnitPrice * Quantity;
    }
}
