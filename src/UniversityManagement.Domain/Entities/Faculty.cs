using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("faculties")]
public class Faculty : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("short_name")]
    public string? ShortName { get; set; }

    [Column("dean_id")]
    public Guid? DeanId { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
