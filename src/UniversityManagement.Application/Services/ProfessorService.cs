using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Service implementation for Professor business logic
/// </summary>
public class ProfessorService : IProfessorService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUserRepository _userRepository;

    public ProfessorService(
        IProfessorRepository professorRepository,
        IUserRepository userRepository)
    {
        _professorRepository = professorRepository;
        _userRepository = userRepository;
    }

    public async Task<ProfessorResponse?> GetByIdAsync(Guid id)
    {
        var professor = await _professorRepository.GetByIdAsync(id);
        if (professor == null)
        {
            return null;
        }

        return await MapToResponseAsync(professor);
    }

    public async Task<ProfessorResponse?> GetByUserIdAsync(Guid userId)
    {
        var professor = await _professorRepository.GetByUserIdAsync(userId);
        if (professor == null)
        {
            return null;
        }

        return await MapToResponseAsync(professor);
    }

    public async Task<List<ProfessorResponse>> GetAllAsync()
    {
        var professors = await _professorRepository.GetAllAsync();
        var responses = new List<ProfessorResponse>();

        foreach (var professor in professors)
        {
            responses.Add(await MapToResponseAsync(professor));
        }

        return responses;
    }

    public async Task<List<ProfessorResponse>> GetByDepartmentAsync(string department)
    {
        var professors = await _professorRepository.GetByDepartmentAsync(department);
        var responses = new List<ProfessorResponse>();

        foreach (var professor in professors)
        {
            responses.Add(await MapToResponseAsync(professor));
        }

        return responses;
    }

    public async Task<List<ProfessorResponse>> GetByFacultyAsync(Guid facultyId)
    {
        var professors = await _professorRepository.GetByFacultyAsync(facultyId);
        var responses = new List<ProfessorResponse>();

        foreach (var professor in professors)
        {
            responses.Add(await MapToResponseAsync(professor));
        }

        return responses;
    }

    public async Task<ProfessorResponse> CreateAsync(CreateProfessorRequest request)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found");
        }

        // Check if professor already exists for this user
        var existingProfessor = await _professorRepository.GetByUserIdAsync(request.UserId);
        if (existingProfessor != null)
        {
            throw new InvalidOperationException($"Professor profile already exists for user {request.UserId}");
        }

        var professor = new Professor
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Title = request.Title,
            Department = request.Department,
            Phone = request.Phone
        };

        var created = await _professorRepository.CreateAsync(professor);
        return await MapToResponseAsync(created);
    }

    public async Task<ProfessorResponse> UpdateAsync(Guid id, UpdateProfessorRequest request)
    {
        var professor = await _professorRepository.GetByIdAsync(id);
        if (professor == null)
        {
            throw new KeyNotFoundException($"Professor with ID {id} not found");
        }

        professor.FirstName = request.FirstName;
        professor.LastName = request.LastName;
        professor.Title = request.Title;
        professor.Department = request.Department;
        professor.Phone = request.Phone;

        var updated = await _professorRepository.UpdateAsync(professor);
        return await MapToResponseAsync(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _professorRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Professor with ID {id} not found");
        }

        await _professorRepository.DeleteAsync(id);
    }

    public async Task<List<ProfessorResponse>> SearchAsync(string searchTerm)
    {
        var professors = await _professorRepository.SearchAsync(searchTerm);
        var responses = new List<ProfessorResponse>();

        foreach (var professor in professors)
        {
            responses.Add(await MapToResponseAsync(professor));
        }

        return responses;
    }

    private async Task<ProfessorResponse> MapToResponseAsync(Professor professor)
    {
        // Try to get user email
        string? email = null;
        try
        {
            var user = await _userRepository.GetByIdAsync(professor.UserId);
            email = user?.Email;
        }
        catch
        {
            // If user not found, email will remain null
        }

        return new ProfessorResponse
        {
            Id = professor.Id,
            UserId = professor.UserId,
            FirstName = professor.FirstName,
            LastName = professor.LastName,
            Title = professor.Title,
            Department = professor.Department,
            Phone = professor.Phone,
            Email = email,
            CourseCount = 0, // TODO: Load from courses when course management is implemented
            CreatedAt = professor.CreatedAt,
            UpdatedAt = professor.UpdatedAt
        };
    }
}
