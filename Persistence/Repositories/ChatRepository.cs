using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ChatRepository(AppDbContext dbContext) : IChatRepository
{
    public Task RemoveAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        dbContext.Chats.Remove(chat);
        return Task.CompletedTask;
    }

    public async Task AddAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        await dbContext.Chats.AddAsync(chat, cancellationToken);
    }

    public async Task<Chat?> GetChatWithMembersByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Chats
            .Where(c => c.Id == id)
            .Include(c => c.Members)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Chat?> GetChatWithMessagesAndMembersByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Chats
            .Where(c => c.Id == id)
            .Include(c => c.Messages)
            .Include(c => c.Members)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
