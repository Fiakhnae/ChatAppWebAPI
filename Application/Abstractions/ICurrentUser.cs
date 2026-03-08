namespace Application.Abstractions;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    public Guid? UserId { get; }

    public string? UserName { get; }

    public string? Email { get; }
}
