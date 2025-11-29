using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("grades")]
public class Grade : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Reference(typeof(Student))]
    public Student? Student { get; set; }

    [Column("subject")]
    public string Subject { get; set; } = string.Empty;

    [Column("value")]
    public int Value { get; set; }

    [Column("semester")]
    public int Semester { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
