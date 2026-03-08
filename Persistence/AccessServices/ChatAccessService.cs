using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.AccessServices;

public class ChatAccessService(
    AppDbContext appDbContext) : IChatAccessService
{
    public async Task<bool> IsMemberAsync(Guid chatId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await appDbContext
            .ChatUsers
            .AsNoTracking()
            .AnyAsync(
                x => x.ChatId == chatId && x.UserId == userId, 
                cancellationToken);
    }
}
