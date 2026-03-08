using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Mediator;

namespace Application.Chats;

public record CreateChatCommand(string Name) : IRequest<Guid>;

public class CreateChat(
    ICurrentUser currentUser,
    IChatRepository chatRepository, 
    IUnitOfWork unitOfWork,
    ITimeProvider timeProvider) : IRequestHandler<CreateChatCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = Chat.Create(request.Name, currentUser.UserId!.Value, timeProvider);

        await chatRepository.AddAsync(chat, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }
}
