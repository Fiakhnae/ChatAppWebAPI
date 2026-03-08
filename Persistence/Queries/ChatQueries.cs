using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Queries;

public class ChatQueries(AppDbContext dbContext) : IChatQueries
{
    public async Task<List<ChatQueryResponse>> GetChatsByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return await (
            from member in dbContext.ChatUsers.AsNoTracking()
            join chat in dbContext.Chats.AsNoTracking()
                on member.ChatId equals chat.Id
            where member.UserId == userId
            select new ChatQueryResponse(
                chat.Id,
                chat.Name,
                chat.CreatedOnUtc,
                dbContext.ChatUsers
                    .Count(x => x.ChatId == chat.Id)
            )
        ).ToListAsync(cancellationToken);
    }

    public async Task<ChatDetailedQueryResponse?> GetDetailedChatInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chat = await dbContext.Chats
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.OwnerId,
                c.CreatedOnUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (chat is null)
            return null;

        var members = await(
            from member in dbContext.Set<ChatMember>().AsNoTracking()
            join user in dbContext.Set<User>().AsNoTracking()
                on member.UserId equals user.Id
            join identity in dbContext.Set<UserIdentity>().AsNoTracking()
                on user.Id equals identity.UserId
            where member.ChatId == id
            select new ChatMemberQueryResponse(
                member.UserId,
                identity.Username.Value,
                member.Role,
                member.JoinedOnUtc
            )
        ).ToListAsync(cancellationToken);

        return new ChatDetailedQueryResponse(
            chat.Id,
            chat.Name,
            chat.OwnerId,
            chat.CreatedOnUtc,
            members
        );
    }
}
