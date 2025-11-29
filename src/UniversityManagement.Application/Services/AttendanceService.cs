using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IStudentRepository _studentRepository;

    public AttendanceService(IAttendanceRepository attendanceRepository, IStudentRepository studentRepository)
    {
        _attendanceRepository = attendanceRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<AttendanceResponse>> GetAllAsync()
    {
        var attendances = await _attendanceRepository.GetAllAsync();
        return await MapToResponses(attendances);
    }

    public async Task<IEnumerable<AttendanceResponse>> GetByStudentIdAsync(Guid studentId)
    {
        var attendances = await _attendanceRepository.GetByStudentIdAsync(studentId);
        return await MapToResponses(attendances);
    }

    public async Task<AttendanceResponse?> GetByIdAsync(Guid id)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(id);
        if (attendance == null) return null;
        return (await MapToResponses(new[] { attendance })).First();
    }

    public async Task<Guid> CreateAsync(CreateAttendanceRequest request)
    {
        var attendance = new Attendance
        {
            StudentId = request.StudentId,
            Subject = request.Subject,
            Date = request.Date,
            IsPresent = request.IsPresent
        };

        return await _attendanceRepository.CreateAsync(attendance);
    }

    public async Task UpdateAsync(Guid id, CreateAttendanceRequest request)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(id);
        if (attendance == null) throw new Exception("Attendance record not found");

        attendance.StudentId = request.StudentId;
        attendance.Subject = request.Subject;
        attendance.Date = request.Date;
        attendance.IsPresent = request.IsPresent;

        await _attendanceRepository.UpdateAsync(attendance);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _attendanceRepository.DeleteAsync(id);
    }

    private async Task<IEnumerable<AttendanceResponse>> MapToResponses(IEnumerable<Attendance> attendances)
    {
        var responses = new List<AttendanceResponse>();
        foreach (var attendance in attendances)
        {
            var student = await _studentRepository.GetByIdAsync(attendance.StudentId);
            responses.Add(new AttendanceResponse
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                Subject = attendance.Subject,
                Date = attendance.Date,
                IsPresent = attendance.IsPresent
            });
        }
        return responses;
    }
}
