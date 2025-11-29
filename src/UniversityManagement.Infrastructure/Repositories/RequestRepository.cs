using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly Client _supabaseClient;

    public RequestRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Request?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient.From<Request>()
            .Where(r => r.Id == id)
            .Single();
        return response;
    }

    public async Task<IEnumerable<Request>> GetByStudentIdAsync(Guid studentId)
    {
        var response = await _supabaseClient.From<Request>()
            .Where(r => r.StudentId == studentId)
            .Get();
        return response.Models;
    }

    public async Task<IEnumerable<Request>> GetAllAsync()
    {
        var response = await _supabaseClient.From<Request>()
            .Get();
        return response.Models;
    }

    public async Task<IEnumerable<Request>> GetByStatusAsync(RequestStatus status)
    {
        var response = await _supabaseClient.From<Request>()
            .Where(r => r.Status == status)
            .Get();
        return response.Models;
    }

    public async Task<Guid> CreateAsync(Request request)
    {
        var response = await _supabaseClient.From<Request>().Insert(request);
        var createdRequest = response.Models.First();
        return createdRequest.Id;
    }

    public async Task UpdateAsync(Request request)
    {
        await _supabaseClient.From<Request>().Update(request);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient.From<Request>().Where(r => r.Id == id).Delete();
    }
}
