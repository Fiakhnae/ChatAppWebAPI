using Domain.Exceptions;

namespace Domain.Errors;

public static class ChatErrors
{
    public static Error NameTooLong(int maxLenght) => new(
        "NameTooLong",
        $"Chat name cannot exceed {maxLenght} characters."
    );

    public static Error NotAllowedToUpdateName(Guid userId, Guid chatId) => new(
        "NotAllowedToUpdateName",
        $"User with ID '{userId}' is not allowed to update the name of chat with ID '{chatId}'."
    );

    public static Error NotAllowedToDeleteChat(Guid userId, Guid chatId) => new(
        "NotAllowedToDeleteChat",
        $"User with ID '{userId}' is not allowed to delete chat with ID '{chatId}'."
    );

    public static Error NotAllowedToAddUser(Guid userId, Guid chatId) => new(
        "NotAllowedToAddUser",
        $"User with ID '{userId}' is not allowed to add users to chat with ID '{chatId}'."
    );

    public static Error NotAllowedToRemoveUser(Guid removerId, Guid userId, Guid chatId) => new(
        "NotAllowedToRemoveUser",
        $"User with ID '{removerId}' is not allowed to remove user with ID '{userId}' from chat with ID '{chatId}'."
    );

    public static Error ChatNotFound(Guid chatId) => new(
        "ChatNotFound",
        $"Chat with ID '{chatId}' was not found."
    );

    public static Error ChatCantBeEmpty(Guid chatId) => new(
        "ChatCantBeEmpty",
        $"Chat with ID '{chatId}' cant be empty.");

    public static Error MessageNotFound(Guid messageId) => new(
        "MessageNotFound",
        $"Message with ID '{messageId}' was not found."
    );

    public static Error UserAlreadyInChat(Guid userId, Guid chatId) => new(
        "UserAlreadyInChat",
        $"User with ID '{userId}' is already a member of chat with ID '{chatId}'."
    );

    public static Error MemberNotFound(Guid memberId) => new(
        "MemberNotFound",
        $"Chat member with ID '{memberId}' was not found."
    );
}
