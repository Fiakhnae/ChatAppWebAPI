using Domain.Entities;

namespace Application.Abstractions;

public record ChatPermissions(
    bool CanAddMembers,
    bool CanLeaveChat,
    bool CanDeleteChat,
    bool CanUpdateName,
    bool CanRemoveMembers
);

public interface IChatPermissionsProvider
{
    Task<(MemberRole, ChatPermissions)> GetPermissionsAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
}
