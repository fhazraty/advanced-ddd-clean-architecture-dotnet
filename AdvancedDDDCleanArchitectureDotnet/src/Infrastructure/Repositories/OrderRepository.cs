using Application.Abstractions;
using Domain.Orders;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Order order, CancellationToken ct)
            => await _db.Orders.AddAsync(order, ct);

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<IReadOnlyList<Order>> ListAsync(ISpecification<Order> spec, CancellationToken ct)
            => await spec.Criteria(_db.Orders.AsQueryable())
                .Include(o => o.Items)
                .ToListAsync(ct);
    }

}
