using Domain.Abstractions;
using Domain.Errors;
using Domain.Exceptions;

namespace Domain.Entities;

public class Chat : Entity
{
    public static readonly int MaxNameLength = 100;

    private List<Message> _messages = [];

    private List<ChatMember> _members = [];

    private Chat() : base(Guid.NewGuid()) { }

    private Chat(string name, Guid ownerId, DateTime createdOnUtc) : base(Guid.NewGuid())
    {
        Name = name;
        OwnerId = ownerId;
        CreatedOnUtc = createdOnUtc;
    }

    public string Name { get; private set; }

    public Guid OwnerId { get; init; }

    public DateTime CreatedOnUtc { get; init; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public IReadOnlyCollection<ChatMember> Members => _members.AsReadOnly();

    public void UpdateName(ChatMember chatMember, string name)
    {
        if (!_members.Contains(chatMember))
        {
            throw new NotFoundException(ChatErrors.ChatNotFound(Id));
        }

        if (!chatMember.IsAdminRole())
        {
            throw new BadRequestException(ChatErrors.NotAllowedToUpdateName(chatMember.UserId, Id));
        }

        Validate(name);

        Name = name;
    }

    public void AddMessage(ChatMember chatMember, string text, ITimeProvider timeProvider)
    {
        if (!_members.Contains(chatMember))
        {
            throw new BadRequestException(ChatErrors.ChatNotFound(Id));
        }

        var message = Message.Create(Id, chatMember.UserId, text, timeProvider);

        _messages.Add(message);
    }

    public void RemoveMessage(Message message)
    {
        if (!_messages.Contains(message))
        {
            throw new NotFoundException(MessageErrors.MessageNotFound(message.Id));
        }

        _messages.Remove(message);
    }

    public void AddMember(ChatMember admin, Guid userId, ITimeProvider timeProvider)
    {
        if (!_members.Contains(admin))
        {
            throw new BadRequestException(ChatErrors.ChatNotFound(Id));
        }

        if (!admin.IsAdminRole())
        {
            throw new BadRequestException(ChatErrors.NotAllowedToAddUser(admin.UserId, Id));
        }

        if (_members.Any(m => m.UserId == userId))
        {
            throw new BadRequestException(ChatErrors.UserAlreadyInChat(userId, Id));
        }

        MemberRole role = MemberRole.Member;

        if (userId == OwnerId)
        {
            role = MemberRole.Owner;
        }

        var member = ChatMember.Create(Id, userId, role, timeProvider);

        _members.Add(member);
    }

    public void RemoveMember(ChatMember remover, ChatMember member)
    {
        if (!_members.Contains(remover))
        {
            throw new BadRequestException(ChatErrors.ChatNotFound(Id));
        }

        if (remover == member)
        {
            _members.Remove(member);
            return;
        }

        if (!_members.Contains(member))
        {
            throw new BadRequestException(UserErrors.UserNotFound(member.UserId));
        }

        if (remover.IsOwnerRole())
        {
            _members.Remove(member);
            return;
        }

        if (remover.Role != MemberRole.Admin || member.Role == MemberRole.Admin)
        {
            throw new BadRequestException(ChatErrors.NotAllowedToRemoveUser(remover.UserId, member.UserId, Id));
        }

        _members.Remove(member);
    }

    private static void Validate(string name)
    {
        var exc = new EntityValidationException();

        if (name.Length >= MaxNameLength)
        {
            exc.AddError(ChatErrors.NameTooLong(MaxNameLength));
        }

        if (exc.HasErrors())
        {
            throw exc;
        }
    }

    public static Chat Create(string name, Guid ownerId, ITimeProvider timeProvider)
    {
        Validate(name);

        var chat = new Chat(name, ownerId, timeProvider.UtcNow);

        chat._members.Add(ChatMember.Create(chat.Id, ownerId, MemberRole.Owner, timeProvider));

        return chat;
    }
}
