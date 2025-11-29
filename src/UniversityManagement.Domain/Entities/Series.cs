using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniversityManagement.Domain.Entities;

[Table("series")]
public class Series : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("program_id")]
    public Guid ProgramId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("year_of_study")]
    public int YearOfStudy { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
