using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using Client = Supabase.Client;

namespace UniversityManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly Client _supabaseClient;

    public StudentRepository(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        var response = await _supabaseClient.From<Student>()
            .Where(s => s.Id == id)
            .Single();
        return response;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        var response = await _supabaseClient.From<Student>()
            .Get();
        return response.Models;
    }

    public async Task<Student?> GetByUserIdAsync(Guid userId)
    {
        var response = await _supabaseClient.From<Student>()
            .Where(s => s.UserId == userId)
            .Single();
        return response;
    }

    public async Task<Guid> CreateAsync(Student student)
    {
        // Insert User first if present and not inserted
        if (student.User != null)
        {
            var userResponse = await _supabaseClient.From<User>().Insert(student.User);
            var createdUser = userResponse.Models.FirstOrDefault();
            if (createdUser != null)
            {
                student.UserId = createdUser.Id;
                // student.User = null; // Avoid circular insert issues if any
            }
        }

        var response = await _supabaseClient.From<Student>().Insert(student);
        var createdStudent = response.Models.First();
        return createdStudent.Id;
    }

    public async Task UpdateAsync(Student student)
    {
        await _supabaseClient.From<Student>().Update(student);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _supabaseClient.From<Student>().Where(s => s.Id == id).Delete();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        // Check in Users table since email is there
        var response = await _supabaseClient.From<User>()
            .Where(u => u.Email == email)
            .Count(Supabase.Postgrest.Constants.CountType.Exact);
        return response > 0;
    }

    public async Task<bool> ExistsByCnpAsync(string cnp)
    {
        var response = await _supabaseClient.From<Student>()
            .Where(s => s.Cnp == cnp)
            .Count(Supabase.Postgrest.Constants.CountType.Exact);
        return response > 0;
    }

    public async Task<bool> ExistsByEnrollmentNumberAsync(string enrollmentNumber)
    {
        var response = await _supabaseClient.From<Student>()
            .Where(s => s.EnrollmentNumber == enrollmentNumber)
            .Count(Supabase.Postgrest.Constants.CountType.Exact);
        return response > 0;
    }
}
