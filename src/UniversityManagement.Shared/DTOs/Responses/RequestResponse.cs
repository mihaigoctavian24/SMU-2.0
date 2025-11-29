using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Responses;

public class RequestResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public RequestType Type { get; set; }
    public RequestStatus Status { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
