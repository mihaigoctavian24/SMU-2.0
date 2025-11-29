using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IStudentService _studentService;

    public AttendanceController(IAttendanceService attendanceService, IStudentService studentService)
    {
        _attendanceService = attendanceService;
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var attendances = await _attendanceService.GetAllAsync();
        return Ok(attendances);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(Guid studentId)
    {
        var attendances = await _attendanceService.GetByStudentIdAsync(studentId);
        return Ok(attendances);
    }

    [HttpGet("my-attendance")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> GetMyAttendance()
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

        var attendances = await _attendanceService.GetByStudentIdAsync(student.Id);
        return Ok(attendances);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var attendance = await _attendanceService.GetByIdAsync(id);
        if (attendance == null) return NotFound();
        return Ok(attendance);
    }

    [HttpPost]
    [Authorize(Roles = "professor,admin")]
    public async Task<IActionResult> Create(CreateAttendanceRequest request)
    {
        try
        {
            var id = await _attendanceService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "professor,admin")]
    public async Task<IActionResult> Update(Guid id, CreateAttendanceRequest request)
    {
        try
        {
            await _attendanceService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "professor,admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _attendanceService.DeleteAsync(id);
        return NoContent();
    }
}
