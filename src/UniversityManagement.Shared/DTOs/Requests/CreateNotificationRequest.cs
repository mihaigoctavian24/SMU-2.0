using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Shared.DTOs.Requests;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}
