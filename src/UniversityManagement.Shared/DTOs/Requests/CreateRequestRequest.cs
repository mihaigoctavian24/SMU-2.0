using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Requests;

public class CreateRequestRequest
{
    public Guid StudentId { get; set; }
    public RequestType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}
