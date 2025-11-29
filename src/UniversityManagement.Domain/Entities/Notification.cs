using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("notifications")]
public class Notification : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("type")]
    public NotificationType Type { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
