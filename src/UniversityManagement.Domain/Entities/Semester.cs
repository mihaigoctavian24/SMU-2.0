using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniversityManagement.Domain.Entities;

[Table("semesters")]
public class Semester : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("academic_year_id")]
    public Guid AcademicYearId { get; set; }

    [Column("number")]
    public int Number { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
