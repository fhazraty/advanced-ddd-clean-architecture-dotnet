using Application.Abstractions;
using Domain.Common;
using MediatR;
using Domain.Orders;

namespace Application.Orders.Commands
{
    public sealed record CreateOrderCommand(string Email) : IRequest<Result<Guid>>;

    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _repo;
        private readonly IUnitOfWork _uow;

        public CreateOrderCommandHandler(IOrderRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            var emailResult = Email.Create(request.Email);
            if (!emailResult.IsSuccess) return Result<Guid>.Failure(emailResult.Error!);

            var orderResult = Order.Create(emailResult.Value!);
            var order = orderResult.Value!;

            await _repo.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<Guid>.Success(order.Id);
        }
    }
}
