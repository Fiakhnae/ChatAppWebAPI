using Application.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Chats;

public record UpdateChatNameCommand(
    Guid ChatId, 
    string Name) : IRequest;

public class UpdateName(ICurrentUser currentUser, IChatRepository chatRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateChatNameCommand>
{
    public async ValueTask<Unit> Handle(UpdateChatNameCommand request, CancellationToken cancellationToken)
    {
        var chat = await chatRepository.GetChatWithMembersByIdAsync(request.ChatId, cancellationToken)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        var member = chat.Members.FirstOrDefault(x => x.UserId == currentUser.UserId)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        chat.UpdateName(member, request.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
