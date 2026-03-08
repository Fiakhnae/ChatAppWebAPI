using Domain.Exceptions;

namespace Domain.Errors;

public static class MessageErrors
{
    public static Error MessageTooLong (int maxLenght) => new(
        "MessageTooLong",
        $"Message text cannot exceed {maxLenght} characters."
    );

    public static Error MessageIsEmpty() => new(
        "MessageIsEmpty",
        "Message text cannot be empty."
    );

    public static Error MessageNotFound(Guid messageId) => new(
        "MessageNotFound",
        $"Message with ID '{messageId}' was not found."
    );
}
