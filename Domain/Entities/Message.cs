using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Message : Entity
{
    public static readonly int MaxTextLength = 4096;

    private Message() : base(Guid.NewGuid()) { }

    private Message(Guid chatId, Guid userId, string text, DateTime createdOnUtc) : base(Guid.NewGuid())
    {
        ChatId = chatId;
        UserId = userId;
        Text = text;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid ChatId { get; init; }

    public Guid UserId { get; init; }

    public string Text { get; private set; }

    public DateTime CreatedOnUtc { get; init; }

    public DateTime? UpdatedOnUtc { get; private set; }

    public void UpdateText(string newText, ITimeProvider timeProvider)
    {
        Text = newText;
        UpdatedOnUtc = timeProvider.UtcNow;
    }

    private static void Validate(string text)
    {
        var exc = new EntityValidationException();

        if (string.IsNullOrWhiteSpace(text))
        {
            exc.AddError(MessageErrors.MessageIsEmpty());
        }

        if (text.Length > MaxTextLength)
        {
            exc.AddError(MessageErrors.MessageTooLong(MaxTextLength));
        }

        if (exc.HasErrors())
        {
            throw exc;
        }
    }

    public static Message Create(Guid chatId, Guid userId, string text, ITimeProvider timeProvider)
    {
        Validate(text);

        return new Message(chatId, userId, text, timeProvider.UtcNow);
    }
}
