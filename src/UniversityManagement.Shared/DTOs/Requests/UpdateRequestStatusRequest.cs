using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Requests;

public class UpdateRequestStatusRequest
{
    public RequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
}
