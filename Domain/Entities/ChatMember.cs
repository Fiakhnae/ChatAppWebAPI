using Domain.Abstractions;

namespace Domain.Entities;

public enum MemberRole
{
    Owner,
    Member,
    Admin
}

public class ChatMember : Entity
{
    private ChatMember() : base(Guid.NewGuid()) { }

    private ChatMember(Guid chatId, Guid userId, MemberRole role, DateTime joinedOnUtc) : base(Guid.NewGuid())
    {
        ChatId = chatId;
        UserId = userId;
        Role = role;
        JoinedOnUtc = joinedOnUtc;
    }

    public Guid ChatId { get; init; }

    public Guid UserId { get; init; }

    public MemberRole Role { get; private set; }

    public DateTime JoinedOnUtc { get; init; }

    public void UpdateRole(MemberRole newRole)
    {
        Role = newRole;
    }

    public bool IsAdminRole()
    {
        return Role == MemberRole.Admin || Role == MemberRole.Owner;
    }

    public bool IsOwnerRole()
    {
        return Role == MemberRole.Owner;
    }

    public static ChatMember Create(Guid chatId, Guid userId, MemberRole role, ITimeProvider timeProvider)
    {
        return new ChatMember(chatId, userId, role, timeProvider.UtcNow);
    }
}
