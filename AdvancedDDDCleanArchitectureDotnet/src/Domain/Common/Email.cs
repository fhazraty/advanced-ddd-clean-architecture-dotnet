namespace Domain.Common
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result<Email>.Failure(new Error("email.empty", "ایمیل خالی است."));

            if (!value.Contains('@') || value.Length < 6)
                return Result<Email>.Failure(new Error("email.invalid", "فرمت ایمیل نامعتبر است."));

            return Result<Email>.Success(new Email(value.Trim().ToLowerInvariant()));
        }

        public override string ToString() => Value;
    }

}
