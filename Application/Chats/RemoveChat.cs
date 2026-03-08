using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Chats;

public record RemoveChatCommand(Guid ChatId) : IRequest;

public class RemoveChat(
    ICurrentUser currentUser,
    IChatRepository chatRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveChatCommand>
{
    public async ValueTask<Unit> Handle(RemoveChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await chatRepository.GetChatWithMembersByIdAsync(request.ChatId, cancellationToken)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        if (chat.OwnerId != currentUser.UserId!.Value)
        {
            throw new BadRequestException(ChatErrors.NotAllowedToDeleteChat(currentUser.UserId!.Value, request.ChatId));
        }

        await chatRepository.RemoveAsync(chat, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
