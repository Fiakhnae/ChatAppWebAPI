using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Chats;

public record ChatDetailedResponse(
    Guid Id,
    string Name,
    Guid OwnerId,
    DateTime CreatedAtUtc,
    MemberRole CurrentUserRole,
    ChatPermissions Permissions,
    List<ChatMemberResponse> Members);

public record ChatMemberResponse(
    Guid UserId,
    string Username,
    MemberRole Role,
    DateTime JoinedAt);

public record GetChatQuery(Guid ChatId) : IRequest<ChatDetailedResponse>;

public class GetChat(
    ICurrentUser currentUser,
    IChatAccessService chatAccessService,
    IChatPermissionsProvider chatPermissionsProvider,
    IChatQueries chatQueries) : IRequestHandler<GetChatQuery, ChatDetailedResponse>
{
    public async ValueTask<ChatDetailedResponse> Handle(GetChatQuery request, CancellationToken cancellationToken)
    {
        if (!await chatAccessService.IsMemberAsync(request.ChatId, currentUser.UserId!.Value, cancellationToken))
        {
            throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));
        }

        var chat = await chatQueries.GetDetailedChatInfoByIdAsync(request.ChatId)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        var permissions = await chatPermissionsProvider.GetPermissionsAsync(
            request.ChatId, 
            currentUser.UserId!.Value, 
            cancellationToken);

        return new ChatDetailedResponse(
            chat.Id,
            chat.Name,
            chat.OwnerId,
            chat.CreatedAtUtc,
            permissions.Item1,
            permissions.Item2,
            chat.Members
                .Select(x => new ChatMemberResponse(
                    x.UserId, x.Username, x.Role, x.JoinedAt))
                .ToList());
    }
}
