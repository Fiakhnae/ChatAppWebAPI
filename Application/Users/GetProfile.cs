using Application.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Users;

public record GetProfileQuery() : IRequest<UserResponse>;

public class GetProfile(
    ICurrentUser currentUser,
    IUserQueries userQueries) : IRequestHandler<GetProfileQuery, UserResponse>
{
    public async ValueTask<UserResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetUserByIdAsync(currentUser.UserId!.Value, cancellationToken)
            ?? throw new BadRequestException(UserErrors.UserNotFound(currentUser.UserId!.Value));

        return user;
    }
}
