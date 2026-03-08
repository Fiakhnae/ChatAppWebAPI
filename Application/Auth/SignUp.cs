using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;
using Mediator;

namespace Application.Auth;

public record SignUpCommand(
    string Email, 
    string Username, 
    string Password, 
    int Gender, 
    DateOnly? birthDate) : IRequest<SignUpResponse>;

public record SignUpResponse(
    Guid Id,
    string Email,
    string Username,
    string SecurityStamp);

public class SignUp(
    ITimeProvider timeProvider, 
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUserIdentityService userIdentityService,
    IUnitOfWork unitOfWork) : IRequestHandler<SignUpCommand, SignUpResponse>
{
    public async ValueTask<SignUpResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (await userIdentityService.ExistsByEmailOrUsernameAsync(request.Email, cancellationToken))
        {
            throw new BadRequestException(UserErrors.UserAlreadyExists);
        }

        var userIdentityId = Guid.NewGuid();

        var user = new User(
            userIdentityId, 
            (Gender)request.Gender, 
            request.birthDate, 
            timeProvider.UtcNow);

        var userIdentity = new UserIdentity(
            userIdentityId,
            user.Id,
            Username.Create(request.Username),
            Email.Create(request.Email), 
            Password.Create(request.Password),
            passwordHasher);

        await userRepository.AddAsync(user, cancellationToken);

        await userIdentityService.AddAsync(userIdentity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SignUpResponse(user.Id, userIdentity.Email, userIdentity.Username, userIdentity.SecurityStamp);
    }
}
