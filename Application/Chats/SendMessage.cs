using Application.Abstractions;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Chats;

public record SendMessageCommand(Guid ChatId, string Message) : IRequest;

public class SendMessage(
    ICurrentUser currentUser,
    IChatRepository chatRepository,
    ITimeProvider timeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<SendMessageCommand>
{
    public async ValueTask<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await chatRepository.GetChatWithMessagesAndMembersByIdAsync(request.ChatId, cancellationToken)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        var user = chat.Members.FirstOrDefault(x => x.UserId == currentUser.UserId)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        chat.AddMessage(user, request.Message, timeProvider);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
