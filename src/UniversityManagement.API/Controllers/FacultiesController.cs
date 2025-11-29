using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacultiesController : ControllerBase
{
    private readonly IFacultyService _facultyService;
    private readonly ILogger<FacultiesController> _logger;

    public FacultiesController(IFacultyService facultyService, ILogger<FacultiesController> logger)
    {
        _facultyService = facultyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<FacultyResponse>>>> GetAll()
    {
        try
        {
            var faculties = await _facultyService.GetAllAsync();
            return Ok(ApiResponse<List<FacultyResponse>>.SuccessResponse(faculties));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculties");
            return StatusCode(500, ApiResponse<List<FacultyResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> GetById(Guid id)
    {
        try
        {
            var faculty = await _facultyService.GetByIdAsync(id);
            if (faculty == null)
            {
                return NotFound(ApiResponse<FacultyResponse>.ErrorResponse("Faculty not found", statusCode: 404));
            }
            return Ok(ApiResponse<FacultyResponse>.SuccessResponse(faculty));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty {Id}", id);
            return StatusCode(500, ApiResponse<FacultyResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,rector")]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> Create([FromBody] CreateFacultyRequest request)
    {
        try
        {
            var faculty = await _facultyService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = faculty.Id }, 
                ApiResponse<FacultyResponse>.SuccessResponse(faculty, "Faculty created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating faculty");
            return StatusCode(500, ApiResponse<FacultyResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,rector")]
    public async Task<ActionResult<ApiResponse<FacultyResponse>>> Update(Guid id, [FromBody] UpdateFacultyRequest request)
    {
        try
        {
            var faculty = await _facultyService.UpdateAsync(id, request);
            return Ok(ApiResponse<FacultyResponse>.SuccessResponse(faculty, "Faculty updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Faculty {Id} not found", id);
            return NotFound(ApiResponse<FacultyResponse>.ErrorResponse("Faculty not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating faculty {Id}", id);
            return StatusCode(500, ApiResponse<FacultyResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _facultyService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Faculty deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Faculty {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Faculty not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting faculty {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<FacultyResponse>>>> Search([FromQuery] string term)
    {
        try
        {
            var faculties = await _facultyService.SearchAsync(term);
            return Ok(ApiResponse<List<FacultyResponse>>.SuccessResponse(faculties));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching faculties");
            return StatusCode(500, ApiResponse<List<FacultyResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
