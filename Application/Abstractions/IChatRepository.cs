using Domain.Entities;

namespace Application.Abstractions;

public interface IChatRepository
{
    Task<Chat?> GetChatWithMembersByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Chat?> GetChatWithMessagesAndMembersByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Chat chat, CancellationToken cancellationToken = default);

    Task RemoveAsync(Chat chat, CancellationToken cancellationToken = default);
}
