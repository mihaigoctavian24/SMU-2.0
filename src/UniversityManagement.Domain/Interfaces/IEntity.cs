namespace UniversityManagement.Domain.Interfaces;

public interface IEntity
{
    Guid Id { get; set; }
}

public interface IAuditableEntity : IEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
