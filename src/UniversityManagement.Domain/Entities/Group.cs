using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace UniversityManagement.Domain.Entities;

[Table("groups")]
public class Group : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("series_id")]
    public Guid SeriesId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
