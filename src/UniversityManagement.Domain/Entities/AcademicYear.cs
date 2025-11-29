using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniversityManagement.Domain.Entities;

[Table("academic_years")]
public class AcademicYear : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("is_current")]
    public bool IsCurrent { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
