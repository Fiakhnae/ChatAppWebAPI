using Application.Abstractions;
using Domain.Exceptions;
using Mediator;

namespace Application.Auth;

public record MeCommand() : IRequest<UserResponse>;

public record UserResponse(Guid Id, string Username);

public class Me(ICurrentUser currentUser) : IRequestHandler<MeCommand, UserResponse>
{
    public async ValueTask<UserResponse> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        return new UserResponse(currentUser.UserId!.Value, currentUser.UserName!);
    }
}
