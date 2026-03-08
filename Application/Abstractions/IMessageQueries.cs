namespace Application.Abstractions;

public record MessageQueryResponse(
    Guid Id,
    Guid SenderId,
    string SenderUsername,
    string Content,
    DateTime SentAtUtc);

public interface IMessageQueries
{
    Task<List<MessageQueryResponse>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken);
}
