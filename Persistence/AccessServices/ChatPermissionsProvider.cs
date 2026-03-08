using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure;

public class ChatPermissionsProvider(AppDbContext dbContext) : IChatPermissionsProvider
{
    public async Task<(MemberRole, ChatPermissions)> GetPermissionsAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
    {
        MemberRole? role = await (from chat in dbContext.Chats.AsNoTracking()
                          join member in dbContext.ChatUsers.AsNoTracking()
                            on chat.Id equals member.ChatId
                          where chat.Id == chatId && member.UserId == userId
                          select member.Role)
                          .FirstOrDefaultAsync(cancellationToken);

        return role switch
        {
            MemberRole.Member => (MemberRole.Member, new ChatPermissions(false, true, false, false, false)),
            MemberRole.Admin => (MemberRole.Admin, new ChatPermissions(true, true, false, true, true)),
            MemberRole.Owner => (MemberRole.Owner, new ChatPermissions(true, true, true, true, true)),
            _ => (MemberRole.Member, new ChatPermissions(false, false, false, false, false))
        };
    }
}
