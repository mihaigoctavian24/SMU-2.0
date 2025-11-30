using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;
    private readonly ILogger<SemestersController> _logger;

    public SemestersController(ISemesterService semesterService, ILogger<SemestersController> logger)
    {
        _semesterService = semesterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SemesterResponse>>>> GetAll()
    {
        try
        {
            var semesters = await _semesterService.GetAllAsync();
            return Ok(ApiResponse<List<SemesterResponse>>.SuccessResponse(semesters));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving semesters");
            return StatusCode(500, ApiResponse<List<SemesterResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(Guid id)
    {
        try
        {
            var semester = await _semesterService.GetByIdAsync(id);
            if (semester == null)
            {
                return NotFound(ApiResponse<SemesterResponse>.ErrorResponse("Semester not found", statusCode: 404));
            }
            return Ok(ApiResponse<SemesterResponse>.SuccessResponse(semester));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving semester {Id}", id);
            return StatusCode(500, ApiResponse<SemesterResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("academic-year/{academicYearId}")]
    public async Task<ActionResult<ApiResponse<List<SemesterResponse>>>> GetByAcademicYearId(Guid academicYearId)
    {
        try
        {
            var semesters = await _semesterService.GetByAcademicYearIdAsync(academicYearId);
            return Ok(ApiResponse<List<SemesterResponse>>.SuccessResponse(semesters));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving semesters for academic year {AcademicYearId}", academicYearId);
            return StatusCode(500, ApiResponse<List<SemesterResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create([FromBody] CreateSemesterRequest request)
    {
        try
        {
            var semester = await _semesterService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = semester.Id },
                ApiResponse<SemesterResponse>.SuccessResponse(semester, "Semester created successfully", 201));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Academic year not found when creating semester");
            return NotFound(ApiResponse<SemesterResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating semester");
            return StatusCode(500, ApiResponse<SemesterResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(Guid id, [FromBody] UpdateSemesterRequest request)
    {
        try
        {
            var semester = await _semesterService.UpdateAsync(id, request);
            return Ok(ApiResponse<SemesterResponse>.SuccessResponse(semester, "Semester updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found when updating semester {Id}", id);
            return NotFound(ApiResponse<SemesterResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating semester {Id}", id);
            return StatusCode(500, ApiResponse<SemesterResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _semesterService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Semester deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Semester {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Semester not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting semester {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
