using Domain.Abstractions;
using Domain.ValueObjects;

namespace Domain.Entities;

public enum Gender
{
    Male,
    Female
}

public class User : Entity
{
    private User() : base(Guid.NewGuid()) { }

    public User(
        Guid identityId,
        Gender gender,
        DateOnly? birthDate, 
        DateTime createdOnUtc) : base(Guid.NewGuid())
    {
        IdentityId = identityId;
        Gender = gender;
        BirthDate = birthDate;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid IdentityId {  get; init; }

    public Gender Gender { get; set; }

    public DateOnly? BirthDate { get; init; }

    public DateTime CreatedOnUtc { get; init; }
}
