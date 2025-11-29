using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain.Entities;

[Table("students")]
public class Student : BaseModel, IAuditableEntity
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("group_id")]
    public Guid? GroupId { get; set; }

    [Column("enrollment_number")]
    public string EnrollmentNumber { get; set; } = string.Empty;

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("cnp")]
    public string? Cnp { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("birth_date")]
    public DateOnly? BirthDate { get; set; }

    [Column("status")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public StudentStatus Status { get; set; } = StudentStatus.Active;

    [Column("enrolled_at")]
    public DateOnly EnrolledAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    [Reference(typeof(User))]
    public User? User { get; set; }
}
