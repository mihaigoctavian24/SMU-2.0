using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly Client _supabaseClient;

    public GradeRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Grade?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient.From<Grade>()
            .Where(g => g.Id == id)
            .Single();
        return response;
    }

    public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId)
    {
        var response = await _supabaseClient.From<Grade>()
            .Where(g => g.StudentId == studentId)
            .Get();
        return response.Models;
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        var response = await _supabaseClient.From<Grade>()
            .Get();
        return response.Models;
    }

    public async Task<Guid> CreateAsync(Grade grade)
    {
        var response = await _supabaseClient.From<Grade>().Insert(grade);
        var createdGrade = response.Models.First();
        return createdGrade.Id;
    }

    public async Task UpdateAsync(Grade grade)
    {
        await _supabaseClient.From<Grade>().Update(grade);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient.From<Grade>().Where(g => g.Id == id).Delete();
    }
}
