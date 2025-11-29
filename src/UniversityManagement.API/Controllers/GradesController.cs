using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IStudentService _studentService;

    public GradesController(IGradeService gradeService, IStudentService studentService)
    {
        _gradeService = gradeService;
        _studentService = studentService;
    }

    [HttpGet("my-grades")]
    [Authorize(Roles = "student")]
    public async Task<IActionResult> GetMyGrades()
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

        var grades = await _gradeService.GetByStudentIdAsync(student.Id);
        return Ok(grades);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var grades = await _gradeService.GetAllAsync();
        return Ok(grades);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(Guid studentId)
    {
        var grades = await _gradeService.GetByStudentIdAsync(studentId);
        return Ok(grades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var grade = await _gradeService.GetByIdAsync(id);
        if (grade == null) return NotFound();
        return Ok(grade);
    }

    [HttpPost]
    [Authorize(Roles = "professor,admin")]
    public async Task<IActionResult> Create(CreateGradeRequest request)
    {
        try
        {
            var id = await _gradeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "professor,admin")]
    public async Task<IActionResult> Update(Guid id, CreateGradeRequest request)
    {
        try
        {
            await _gradeService.UpdateAsync(id, request);
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
        await _gradeService.DeleteAsync(id);
        return NoContent();
    }
}
