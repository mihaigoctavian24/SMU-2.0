using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Enums;
using UniversityManagement.Shared.DTOs.Requests;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;
    private readonly IStudentService _studentService;
    private readonly IPdfGenerationService _pdfService;

    public RequestsController(IRequestService requestService, IStudentService studentService, IPdfGenerationService pdfService)
    {
        _requestService = requestService;
        _studentService = studentService;
        _pdfService = pdfService;
    }

    [HttpGet("{id}/document")]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var request = await _requestService.GetByIdAsync(id);
        if (request == null) return NotFound();

        if (request.Status != RequestStatus.Approved && request.Status != RequestStatus.Completed)
        {
            return BadRequest("Document is not available for this request status.");
        }

        var pdfBytes = _pdfService.GenerateRequestDocument(request);
        return File(pdfBytes, "application/pdf", $"Request_{request.Type}_{request.CreatedAt:yyyyMMdd}.pdf");
    }

    [HttpGet]
    [Authorize(Roles = "admin,secretariat")]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _requestService.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("my-requests")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var student = await _studentService.GetByUserIdAsync(userId);
        if (student == null)
        {
            return NotFound("Student profile not found for current user.");
        }

        var requests = await _requestService.GetByStudentIdAsync(student.Id);
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var request = await _requestService.GetByIdAsync(id);
        if (request == null) return NotFound();
        return Ok(request);
    }

    [HttpPost]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> Create(CreateRequestRequest request)
    {
        try
        {
            // Ensure student is creating request for themselves
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null || student.Id != request.StudentId)
            {
                return BadRequest("Invalid student ID.");
            }

            var id = await _requestService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "admin,secretariat")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateRequestStatusRequest request)
    {
        try
        {
            await _requestService.UpdateStatusAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,secretariat")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _requestService.DeleteAsync(id);
        return NoContent();
    }
}
