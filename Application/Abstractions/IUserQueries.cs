using Domain.Entities;

namespace Application.Abstractions;

public record UserResponse(
    Guid Id,
    string Email,
    string Username,
    Gender Gender,
    DateOnly? BirthDate,
    DateTime CreatedOnUtc);

public interface IUserQueries
{
    Task<UserResponse?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
