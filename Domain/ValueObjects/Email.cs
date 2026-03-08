using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Email
{
    public static readonly int MaxEmailLength = 254;

    public static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

    public static implicit operator string(Email email) => email.Value;

    public Email(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public static Email Create(string email)
    {
        var exc = new EntityValidationException();

        if (string.IsNullOrWhiteSpace(email))
        {
            exc.AddError(EmailErrors.EmailIsEmpty());
        }

        if (email.Length > MaxEmailLength)
        {
            exc.AddError(EmailErrors.EmailTooLong(MaxEmailLength));
        }

        if (!EmailRegex.IsMatch(email))
        {
            exc.AddError(EmailErrors.EmailHasInvalidFormat());
        }

        if (exc.HasErrors())
        {
            throw exc;
        }

        return new Email(email);
    }
}
