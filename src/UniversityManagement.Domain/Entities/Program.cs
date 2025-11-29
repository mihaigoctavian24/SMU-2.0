using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniversityManagement.Domain.Entities;

[Table("programs")]
public class Program : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("faculty_id")]
    public Guid FacultyId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("degree_level")]
    public string DegreeLevel { get; set; } = string.Empty;

    [Column("duration_years")]
    public int DurationYears { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
