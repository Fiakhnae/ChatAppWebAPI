using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Password
{
    public static readonly int MinPasswordLength = 8;

    public static readonly int MaxPasswordLength = 64;

    public static readonly Regex PasswordRegex = new(@"^(?=.*[A-Za-z])(?=.*\d).{8,64}$");

    public static implicit operator string(Password password) => password.Value;

    internal Password(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Password Create(string password)
    {
        var exc = new EntityValidationException();

        if (string.IsNullOrWhiteSpace(password))
        {
            exc.AddError(PasswordErrors.PasswordIsEmpty());
        }

        if (password.Length < MinPasswordLength)
        {
            exc.AddError(PasswordErrors.PasswordTooShort(MinPasswordLength));
        }

        if (password.Length > MaxPasswordLength)
        {
            exc.AddError(PasswordErrors.PasswordTooLong(MaxPasswordLength));
        }

        if (!PasswordRegex.IsMatch(password))
        {
            exc.AddError(PasswordErrors.PasswordHasInvalidFormat());
        }

        if (exc.HasErrors())
        {
            throw exc;
        }
        return new Password(password);
    }
}
