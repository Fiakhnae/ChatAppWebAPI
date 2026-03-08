using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Queries;

public class MessageQueries(
    AppDbContext dbContext) : IMessageQueries
{
    public async Task<List<MessageQueryResponse>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken)
    {
        return await (
            from message in dbContext.Messages.AsNoTracking()
            join user in dbContext.Users.AsNoTracking()
                on message.UserId equals user.Id
            join identity in dbContext.UserIdentities.AsNoTracking()
                on user.IdentityId equals identity.Id
            where message.ChatId == chatId
            orderby message.CreatedOnUtc
            select new MessageQueryResponse(
                message.Id,
                message.UserId,
                identity.Username,
                message.Text,
                message.CreatedOnUtc))
            .ToListAsync(cancellationToken);
    }
}
