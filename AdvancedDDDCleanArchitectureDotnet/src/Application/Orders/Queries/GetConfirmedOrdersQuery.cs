using Application.Abstractions;
using Domain.Orders;
using MediatR;


namespace Application.Orders.Queries
{
    public sealed record GetConfirmedOrdersQuery : IRequest<IReadOnlyList<Order>>;

    public sealed class ConfirmedOrdersSpec : ISpecification<Order>
    {
        public Func<IQueryable<Order>, IQueryable<Order>> Criteria => q => q.Where(o => o.Status == OrderStatus.Confirmed);
    }

    public sealed class GetConfirmedOrdersQueryHandler : IRequestHandler<GetConfirmedOrdersQuery, IReadOnlyList<Order>>
    {
        private readonly IOrderRepository _repo;

        public GetConfirmedOrdersQueryHandler(IOrderRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<Order>> Handle(GetConfirmedOrdersQuery request, CancellationToken ct)
            => await _repo.ListAsync(new ConfirmedOrdersSpec(), ct);
    }
}
