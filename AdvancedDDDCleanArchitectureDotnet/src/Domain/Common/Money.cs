namespace Domain.Common
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Result<Money> Create(decimal amount, string currency)
        {
            if (amount < 0)
                return Result<Money>.Failure(new Error("money.negative", "مبلغ نمی‌تواند منفی باشد."));

            if (string.IsNullOrWhiteSpace(currency) || currency.Length is < 3 or > 5)
                return Result<Money>.Failure(new Error("money.currency", "واحد پول نامعتبر است."));

            return Result<Money>.Success(new Money(decimal.Round(amount, 2), currency.ToUpperInvariant()));
        }

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency) throw new InvalidOperationException("Currency mismatch");
            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator *(Money a, int qty)
            => new Money(a.Amount * qty, a.Currency);
    }
}
