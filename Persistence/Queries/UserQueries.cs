using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Queries;

public class UserQueries(AppDbContext dbContext) : IUserQueries
{
    public async Task<UserResponse?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await (from identity in dbContext.UserIdentities.AsNoTracking()
                      join userEntity in dbContext.Users.AsNoTracking()
                        on identity.UserId equals userEntity.Id
                      where identity.UserId == id
                      select new UserResponse(
                        userEntity.Id,
                        identity.Email.Value,
                        identity.Username.Value,
                        userEntity.Gender,
                        userEntity.BirthDate,
                        userEntity.CreatedOnUtc))
                    .FirstOrDefaultAsync(cancellationToken);

    }
}
