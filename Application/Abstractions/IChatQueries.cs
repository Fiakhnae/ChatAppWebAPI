using Domain.Entities;

namespace Application.Abstractions;

public record ChatDetailedQueryResponse(
    Guid Id,
    string Name,
    Guid OwnerId,
    DateTime CreatedAtUtc,
    List<ChatMemberQueryResponse> Members);

public record ChatMemberQueryResponse(
    Guid UserId,
    string Username,
    MemberRole Role,
    DateTime JoinedAt);

public record ChatQueryResponse(
    Guid Id,
    string Name,
    DateTime CreatedAtUtc,
    int ChatMembersCount);

public interface IChatQueries
{
    Task<ChatDetailedQueryResponse?> GetDetailedChatInfoByIdAsync(Guid Id, CancellationToken cancellationToken = default);

    Task<List<ChatQueryResponse>> GetChatsByUserId(Guid userId, CancellationToken cancellationToken = default);
}
