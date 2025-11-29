using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("users")]
public class User : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("supabase_auth_id")]
    public Guid SupabaseAuthId { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("role")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public UserRole Role { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
