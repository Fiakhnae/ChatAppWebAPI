using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Authorization;

public class UserIdentityService(AppDbContext dbContext) : IUserIdentityService
{
    public async Task AddAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        await dbContext.UserIdentities.AddAsync(userIdentity, cancellationToken);
    }

    public async Task<bool> ExistsByEmailOrUsernameAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserIdentities.AnyAsync(ui => ui.Email.Value == email || ui.Username.Value == email, cancellationToken);
    }

    public async Task<UserIdentity?> FindByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserIdentities
            .FirstOrDefaultAsync(ui => ui.Email.Value == emailOrUsername || ui.Username.Value == emailOrUsername, cancellationToken);
    }

    public async Task<UserIdentity?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserIdentities.FirstOrDefaultAsync(ui => ui.UserId == userId, cancellationToken);
    }

    public async Task RemoveAsync(UserIdentity userIdentity, CancellationToken cancellationToken = default)
    {
        await dbContext.UserIdentities.Where(ui => ui.UserId == userIdentity.UserId).ExecuteDeleteAsync(cancellationToken);
    }
}
