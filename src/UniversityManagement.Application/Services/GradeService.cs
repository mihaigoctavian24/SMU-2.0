using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IStudentRepository _studentRepository;

    public GradeService(IGradeRepository gradeRepository, IStudentRepository studentRepository)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<GradeResponse>> GetAllAsync()
    {
        var grades = await _gradeRepository.GetAllAsync();
        return await MapToResponses(grades);
    }

    public async Task<IEnumerable<GradeResponse>> GetByStudentIdAsync(Guid studentId)
    {
        var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
        return await MapToResponses(grades);
    }

    public async Task<GradeResponse?> GetByIdAsync(Guid id)
    {
        var grade = await _gradeRepository.GetByIdAsync(id);
        if (grade == null) return null;
        return (await MapToResponses(new[] { grade })).First();
    }

    public async Task<Guid> CreateAsync(CreateGradeRequest request)
    {
        var grade = new Grade
        {
            StudentId = request.StudentId,
            Subject = request.Subject,
            Value = request.Value,
            Semester = request.Semester,
            Date = request.Date
        };

        return await _gradeRepository.CreateAsync(grade);
    }

    public async Task UpdateAsync(Guid id, CreateGradeRequest request)
    {
        var grade = await _gradeRepository.GetByIdAsync(id);
        if (grade == null) throw new Exception("Grade not found");

        grade.StudentId = request.StudentId;
        grade.Subject = request.Subject;
        grade.Value = request.Value;
        grade.Semester = request.Semester;
        grade.Date = request.Date;

        await _gradeRepository.UpdateAsync(grade);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _gradeRepository.DeleteAsync(id);
    }

    private async Task<IEnumerable<GradeResponse>> MapToResponses(IEnumerable<Grade> grades)
    {
        var responses = new List<GradeResponse>();
        foreach (var grade in grades)
        {
            var student = await _studentRepository.GetByIdAsync(grade.StudentId);
            responses.Add(new GradeResponse
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                Subject = grade.Subject,
                Value = grade.Value,
                Semester = grade.Semester,
                Date = grade.Date
            });
        }
        return responses;
    }
}
