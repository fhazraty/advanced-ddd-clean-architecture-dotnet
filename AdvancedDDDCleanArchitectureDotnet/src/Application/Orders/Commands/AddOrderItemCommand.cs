using Application.Abstractions;
using Domain.Common;
using MediatR;

namespace Application.Orders.Commands
{
    public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    decimal UnitPrice,
    string Currency,
    int Quantity
) : IRequest<Result>;

    public sealed class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result>
    {
        private readonly IOrderRepository _repo;
        private readonly IUnitOfWork _uow;

        public AddOrderItemCommandHandler(IOrderRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(AddOrderItemCommand request, CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(request.OrderId, ct);
            if (order is null)
                return Result.Failure(new Error("order.notfound", "سفارش یافت نشد."));

            var moneyResult = Money.Create(request.UnitPrice, request.Currency);
            if (!moneyResult.IsSuccess)
                return Result.Failure(moneyResult.Error!);

            return await AddItemAndSave(order, request, moneyResult.Value!, ct);
        }

        private async Task<Result> AddItemAndSave(Domain.Orders.Order order, AddOrderItemCommand request, Money unitPrice, CancellationToken ct)
        {
            var result = order.AddItem(request.ProductId, unitPrice, request.Quantity);
            if (!result.IsSuccess) return result;

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
