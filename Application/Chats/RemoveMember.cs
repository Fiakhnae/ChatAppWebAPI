using Application.Abstractions;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Chats;

public record RemoveMemberCommand(
    Guid ChatId, 
    Guid UserId) : IRequest;

public class RemoveMember(
    ICurrentUser currentUser,
    IChatRepository chatRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveMemberCommand>
{
    public async ValueTask<Unit> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var chat = await chatRepository.GetChatWithMembersByIdAsync(request.ChatId, cancellationToken)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        var member = chat.Members.FirstOrDefault(m => m.UserId == request.UserId)
            ?? throw new NotFoundException(ChatErrors.MemberNotFound(request.UserId));

        var admin = chat.Members.FirstOrDefault(m => m.UserId == currentUser.UserId)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));
        
        chat.RemoveMember(admin, member);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (chat.Members.Count < 1)
        {
            await chatRepository.RemoveAsync(chat, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
