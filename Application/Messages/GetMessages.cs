using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Messages;

public record MessageResponse(
    Guid Id,
    Guid SenderId,
    string SenderUsername,
    string Content,
    DateTime SentAtUtc);

public record GetMessagesQuery(Guid ChatId) : IRequest<List<MessageResponse>>;

public class GetMessages(
    IChatAccessService chatAccessService, 
    ICurrentUser currentUser, 
    IMessageQueries messageQueries) : IRequestHandler<GetMessagesQuery, List<MessageResponse>>
{
    public async ValueTask<List<MessageResponse>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        if (!await chatAccessService.IsMemberAsync(request.ChatId, currentUser.UserId!.Value, cancellationToken))
        {
            throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));
        }

        var messages = await messageQueries.GetMessagesByChatIdAsync(request.ChatId, cancellationToken);

        return messages
            .Select(x => new MessageResponse(
                x.Id, 
                x.SenderId, 
                x.SenderUsername, 
                x.Content, 
                x.SentAtUtc))
            .ToList();
    }
}
