using Application.Abstractions;
using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;
using Mediator;

namespace Application.Auth;

public record SignInCommand(
    string EmailOrUsername, 
    string Password) : IRequest<SignInResponse>;

public record SignInResponse(
    Guid Id, 
    string Email, 
    string Username,
    string SecurityStamp);

public class SignIn(
    IUserIdentityService userIdentityService,
    IPasswordHasher passwordHasher) : IRequestHandler<SignInCommand, SignInResponse>
{
    public async ValueTask<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var userIdentity = await userIdentityService.FindByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken) 
            ?? throw new NotFoundException(UserErrors.UserNotFoundByEmailOrUsername);

        if (!passwordHasher.VerifyPassword(request.Password, userIdentity.PasswordHash))
        {
            throw new BadRequestException(UserErrors.WrongPassword);
        }

        return new SignInResponse(userIdentity.UserId, userIdentity.Email, userIdentity.Username, userIdentity.SecurityStamp);
    }
}
