using Domain.Orders;

namespace Application.Abstractions
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken ct);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<IReadOnlyList<Order>> ListAsync(ISpecification<Order> spec, CancellationToken ct);
    }
}
