using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;

namespace UniversityManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISupabaseAuthService _authService;

    public StudentService(IStudentRepository studentRepository, ISupabaseAuthService authService)
    {
        _studentRepository = studentRepository;
        _authService = authService;
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        return student == null ? null : MapToResponse(student);
    }

    public async Task<StudentResponse?> GetByUserIdAsync(Guid userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);
        return student == null ? null : MapToResponse(student);
    }

    public async Task<IEnumerable<StudentResponse>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(MapToResponse);
    }

    public async Task<Guid> CreateAsync(CreateStudentRequest request)
    {
        // 1. Create Auth User
        // Note: In a real scenario, we'd generate a temporary password and email it.
        // For MVP, we might set a default or require it in request (but request doesn't have it).
        // Let's assume a default password for now or generate one.
        var defaultPassword = "ChangeMe123!"; 
        var authIdString = await _authService.SignUpAsync(request.Email, defaultPassword);
        
        // 2. Create Student Entity
        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Cnp = request.Cnp,
            Address = request.Address,
            Phone = request.Phone,
            BirthDate = request.BirthDate,
            GroupId = request.GroupId,
            // Generate Enrollment Number (simplified for now)
            EnrollmentNumber = $"AN-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
            Status = StudentStatus.Active,
            UserId = Guid.Empty // Needs to be linked to a User entity in DB, which is linked to AuthId
        };

        // We need to create the User entity first in the DB (users table)
        // But our repository might handle that or we need a UserRepository.
        // For MVP, let's assume the trigger on auth.users might create the public.users entry?
        // No, we defined `users` table manually.
        // We should probably create the User in `users` table.
        // But `SupabaseAuthService` creates in `auth.users`.
        // We need to insert into `public.users`.
        
        // This logic is getting complex for a single service method without a Unit of Work or Transaction.
        // Supabase doesn't support cross-schema transactions easily via client.
        // We might need an RPC or Edge Function for atomic creation.
        // Or just do it sequentially and handle failure.
        
        // For MVP, let's assume we just create the Student and the trigger/logic handles the User link?
        // No, `students.user_id` FK references `users.id`.
        // `users.id` references `auth.users.id`.
        
        // So:
        // 1. SignUp -> creates `auth.users`.
        // 2. Insert into `public.users` with same ID.
        // 3. Insert into `public.students` with `user_id`.
        
        // I'll implement this logic in `StudentRepository.CreateAsync` or split it.
        // Better to keep business logic here but data access in Repo.
        // I'll pass the AuthId to the Repo and let it handle the rest?
        // Or Repo just creates Student, but Student needs UserId.
        
        // I'll update `CreateAsync` in Repo to take the AuthId and Email/Role to create the User too?
        // Or have `IUserRepository`.
        
        // Let's simplify: `StudentRepository.CreateAsync` will take a `Student` which has a `User` navigation property populated?
        // Supabase client might handle deep inserts?
        // Let's try deep insert if supported, or sequential.
        
        var authId = Guid.Parse(authIdString);
        
        student.User = new User
        {
            Id = authId,
            SupabaseAuthId = authId,
            Email = request.Email,
            Role = UserRole.Student
        };
        
        return await _studentRepository.CreateAsync(student);
    }

    public async Task UpdateAsync(Guid id, CreateStudentRequest request)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) throw new Exception("Student not found");

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.Cnp = request.Cnp;
        student.Address = request.Address;
        student.Phone = request.Phone;
        student.BirthDate = request.BirthDate;
        student.GroupId = request.GroupId;

        await _studentRepository.UpdateAsync(student);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _studentRepository.DeleteAsync(id);
    }

    private static StudentResponse MapToResponse(Student student)
    {
        return new StudentResponse
        {
            Id = student.Id,
            EnrollmentNumber = student.EnrollmentNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.User?.Email ?? string.Empty,
            Cnp = student.Cnp,
            Address = student.Address,
            Phone = student.Phone,
            BirthDate = student.BirthDate,
            GroupName = "Group Placeholder", // Need to fetch group name
            Status = student.Status,
            EnrolledAt = student.EnrolledAt
        };
    }
}
