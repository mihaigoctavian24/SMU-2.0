using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<NotificationResponse>> GetUnreadByUserIdAsync(Guid userId);
    Task CreateAsync(CreateNotificationRequest request);
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync(Guid userId);
}
