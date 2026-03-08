namespace Application.Abstractions;

public interface IChatAccessService
{
    Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default);
}
