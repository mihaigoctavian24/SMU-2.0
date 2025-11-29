using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly Client _supabaseClient;

    public AttendanceRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient.From<Attendance>()
            .Where(a => a.Id == id)
            .Single();
        return response;
    }

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId)
    {
        var response = await _supabaseClient.From<Attendance>()
            .Where(a => a.StudentId == studentId)
            .Get();
        return response.Models;
    }

    public async Task<IEnumerable<Attendance>> GetAllAsync()
    {
        var response = await _supabaseClient.From<Attendance>()
            .Get();
        return response.Models;
    }

    public async Task<Guid> CreateAsync(Attendance attendance)
    {
        var response = await _supabaseClient.From<Attendance>().Insert(attendance);
        var createdAttendance = response.Models.First();
        return createdAttendance.Id;
    }

    public async Task UpdateAsync(Attendance attendance)
    {
        await _supabaseClient.From<Attendance>().Update(attendance);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient.From<Attendance>().Where(a => a.Id == id).Delete();
    }
}
