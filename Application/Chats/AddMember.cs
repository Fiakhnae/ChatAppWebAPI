using Application.Abstractions;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Chats;

public record AddMemberCommand(
    Guid ChatId, 
    string Username) : IRequest;

public class AddMember(
    ICurrentUser currentUser, 
    IUserIdentityService userIdentityService,
    IUserRepository userRepository, 
    IChatRepository chatRepository, 
    ITimeProvider timeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<AddMemberCommand>
{
    public async ValueTask<Unit> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var chat = await chatRepository.GetChatWithMembersByIdAsync(request.ChatId, cancellationToken)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        var userIdentity = await userIdentityService.FindByEmailOrUsernameAsync(request.Username, cancellationToken)
            ?? throw new NotFoundException(UserErrors.UserNotFoundByEmailOrUsername);

        var user = await userRepository.GetUserByIdAsync(userIdentity.UserId, cancellationToken)
            ?? throw new NotFoundException(UserErrors.UserNotFound(userIdentity.UserId));

        var admin = chat.Members.FirstOrDefault(m => m.UserId == currentUser.UserId)
            ?? throw new NotFoundException(ChatErrors.ChatNotFound(request.ChatId));

        chat.AddMember(admin, user.Id, timeProvider);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
