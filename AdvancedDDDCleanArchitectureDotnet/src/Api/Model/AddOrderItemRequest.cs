namespace Api.Model
{
    public sealed record AddOrderItemRequest(Guid ProductId, decimal UnitPrice, string Currency, int Quantity);

}
