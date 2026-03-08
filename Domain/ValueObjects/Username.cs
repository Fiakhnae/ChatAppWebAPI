using Domain.Errors;
using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public record Username
{
    public static readonly int MinUsernameLength = 3;

    public static readonly int MaxUsernameLength = 30;

    public static readonly Regex UsernameRegex = new(@"^(?=.{3,30}$)[A-Za-z0-9]+(?:[._-][A-Za-z0-9]+)*$");

    public static implicit operator string(Username username) => username.Value;

    internal Username(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Username Create(string username)
    {
        var exc = new EntityValidationException();

        if (string.IsNullOrWhiteSpace(username))
        {
            exc.AddError(UsernameErrors.UsernameIsEmpty());
        }

        if (username.Length < MinUsernameLength)
        {
            exc.AddError(UsernameErrors.UsernameTooShort(MinUsernameLength));
        }

        if (username.Length > MaxUsernameLength)
        {
            exc.AddError(UsernameErrors.UsernameTooLong(MaxUsernameLength));
        }

        if (!UsernameRegex.IsMatch(username))
        {
            exc.AddError(UsernameErrors.UsernameHasInvalidFormat());
        }

        if (exc.HasErrors())
        {
            throw exc;
        }

        return new Username(username);
    }
}
