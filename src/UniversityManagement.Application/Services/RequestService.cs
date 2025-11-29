using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IStudentRepository _studentRepository;

    public RequestService(IRequestRepository requestRepository, IStudentRepository studentRepository)
    {
        _requestRepository = requestRepository;
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<RequestResponse>> GetAllAsync()
    {
        var requests = await _requestRepository.GetAllAsync();
        return await MapToResponses(requests);
    }

    public async Task<IEnumerable<RequestResponse>> GetByStudentIdAsync(Guid studentId)
    {
        var requests = await _requestRepository.GetByStudentIdAsync(studentId);
        return await MapToResponses(requests);
    }

    public async Task<IEnumerable<RequestResponse>> GetByStatusAsync(RequestStatus status)
    {
        var requests = await _requestRepository.GetByStatusAsync(status);
        return await MapToResponses(requests);
    }

    public async Task<RequestResponse?> GetByIdAsync(Guid id)
    {
        var request = await _requestRepository.GetByIdAsync(id);
        if (request == null) return null;
        return (await MapToResponses(new[] { request })).First();
    }

    public async Task<Guid> CreateAsync(CreateRequestRequest request)
    {
        var newRequest = new Request
        {
            StudentId = request.StudentId,
            Type = request.Type,
            Content = request.Content,
            Status = RequestStatus.Pending
        };

        return await _requestRepository.CreateAsync(newRequest);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateRequestStatusRequest request)
    {
        var existingRequest = await _requestRepository.GetByIdAsync(id);
        if (existingRequest == null) throw new Exception("Request not found");

        existingRequest.Status = request.Status;
        existingRequest.RejectionReason = request.RejectionReason;

        await _requestRepository.UpdateAsync(existingRequest);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _requestRepository.DeleteAsync(id);
    }

    private async Task<IEnumerable<RequestResponse>> MapToResponses(IEnumerable<Request> requests)
    {
        var responses = new List<RequestResponse>();
        foreach (var req in requests)
        {
            var student = await _studentRepository.GetByIdAsync(req.StudentId);
            responses.Add(new RequestResponse
            {
                Id = req.Id,
                StudentId = req.StudentId,
                StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown",
                Type = req.Type,
                Status = req.Status,
                Content = req.Content,
                RejectionReason = req.RejectionReason,
                CreatedAt = req.CreatedAt
            });
        }
        return responses;
    }
}
