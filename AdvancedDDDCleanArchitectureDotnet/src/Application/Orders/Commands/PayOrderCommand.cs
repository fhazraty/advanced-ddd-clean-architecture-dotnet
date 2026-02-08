using Application.Abstractions;
using Domain.Common;
using MediatR;


namespace Application.Orders.Commands
{
    public sealed record PayOrderCommand(Guid OrderId, string PaymentRef) : IRequest<Result>;
    public sealed class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Result>
    {
        private readonly IOrderRepository _repo;
        private readonly IUnitOfWork _uow;

        public PayOrderCommandHandler(IOrderRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(PayOrderCommand request, CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(request.OrderId, ct);
            if (order is null) return Result.Failure(new Error("order.notfound", "سفارش یافت نشد."));

            var result = order.MarkPaid(request.PaymentRef);
            if (!result.IsSuccess) return result;

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
