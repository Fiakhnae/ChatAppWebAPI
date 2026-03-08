using Application.Abstractions;
using Mediator;

namespace Application.Chats;

public record ChatResponse(
    Guid Id,
    string Name,
    DateTime CreatedAtUtc,
    int ChatMembersCount);

public record GetChatsQuery : IRequest<List<ChatResponse>>;

public class GetChats(
    ICurrentUser currentUser,
    IChatQueries chatQueries) : IRequestHandler<GetChatsQuery, List<ChatResponse>>
{
    public async ValueTask<List<ChatResponse>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await chatQueries.GetChatsByUserId(currentUser.UserId!.Value, cancellationToken);

        return chats.Select(x => new ChatResponse(x.Id, x.Name, x.CreatedAtUtc, x.ChatMembersCount)).ToList();
    }
}
