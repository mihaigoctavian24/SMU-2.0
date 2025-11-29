using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("attendance")]
public class Attendance : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Reference(typeof(Student))]
    public Student? Student { get; set; }

    [Column("subject")]
    public string Subject { get; set; } = string.Empty;

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("is_present")]
    public bool IsPresent { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
