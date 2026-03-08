using Domain.Entities;

namespace Application.Abstractions;

public interface IUserIdentityService
{
    Task<bool> ExistsByEmailOrUsernameAsync(string email, CancellationToken cancellationToken = default);

    Task<UserIdentity?> FindByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default);

    Task<UserIdentity?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default);

    Task RemoveAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default);
}
