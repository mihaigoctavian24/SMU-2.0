using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly Client _supabaseClient;

    public NotificationRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseClient.From<Notification>()
            .Where(n => n.UserId == userId)
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
    {
        var response = await _supabaseClient.From<Notification>()
            .Where(n => n.UserId == userId && n.IsRead == false)
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task CreateAsync(Notification notification)
    {
        await _supabaseClient.From<Notification>().Insert(notification);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await _supabaseClient.From<Notification>()
            .Where(n => n.Id == id)
            .Set(n => n.IsRead, true)
            .Update();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _supabaseClient.From<Notification>()
            .Where(n => n.UserId == userId && n.IsRead == false)
            .Set(n => n.IsRead, true)
            .Update();
    }
}
