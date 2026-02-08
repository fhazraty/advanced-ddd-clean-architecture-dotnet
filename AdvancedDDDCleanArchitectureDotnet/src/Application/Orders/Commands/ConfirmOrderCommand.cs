using Application.Abstractions;
using Domain.Common;
using MediatR;

namespace Application.Orders.Commands
{
    public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest<Result>;
    public sealed class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
    {
        private readonly IOrderRepository _repo;
        private readonly IUnitOfWork _uow;

        public ConfirmOrderCommandHandler(IOrderRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(request.OrderId, ct);
            if (order is null) return Result.Failure(new Error("order.notfound", "سفارش یافت نشد."));

            var result = order.Confirm();
            if (!result.IsSuccess) return result;

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
