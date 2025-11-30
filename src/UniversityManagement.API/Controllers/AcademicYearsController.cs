using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicYearsController : ControllerBase
{
    private readonly IAcademicYearService _academicYearService;
    private readonly ILogger<AcademicYearsController> _logger;

    public AcademicYearsController(IAcademicYearService academicYearService, ILogger<AcademicYearsController> logger)
    {
        _academicYearService = academicYearService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AcademicYearResponse>>>> GetAll()
    {
        try
        {
            var academicYears = await _academicYearService.GetAllAsync();
            return Ok(ApiResponse<List<AcademicYearResponse>>.SuccessResponse(academicYears));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving academic years");
            return StatusCode(500, ApiResponse<List<AcademicYearResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> GetById(Guid id)
    {
        try
        {
            var academicYear = await _academicYearService.GetByIdAsync(id);
            if (academicYear == null)
            {
                return NotFound(ApiResponse<AcademicYearResponse>.ErrorResponse("Academic year not found", statusCode: 404));
            }
            return Ok(ApiResponse<AcademicYearResponse>.SuccessResponse(academicYear));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving academic year {Id}", id);
            return StatusCode(500, ApiResponse<AcademicYearResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> GetCurrent()
    {
        try
        {
            var academicYear = await _academicYearService.GetCurrentAsync();
            if (academicYear == null)
            {
                return NotFound(ApiResponse<AcademicYearResponse>.ErrorResponse("No current academic year found", statusCode: 404));
            }
            return Ok(ApiResponse<AcademicYearResponse>.SuccessResponse(academicYear));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current academic year");
            return StatusCode(500, ApiResponse<AcademicYearResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,dean")]
    public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> Create([FromBody] CreateAcademicYearRequest request)
    {
        try
        {
            var academicYear = await _academicYearService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = academicYear.Id },
                ApiResponse<AcademicYearResponse>.SuccessResponse(academicYear, "Academic year created successfully", 201));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating academic year");
            return StatusCode(500, ApiResponse<AcademicYearResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,dean")]
    public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> Update(Guid id, [FromBody] UpdateAcademicYearRequest request)
    {
        try
        {
            var academicYear = await _academicYearService.UpdateAsync(id, request);
            return Ok(ApiResponse<AcademicYearResponse>.SuccessResponse(academicYear, "Academic year updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Academic year {Id} not found", id);
            return NotFound(ApiResponse<AcademicYearResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating academic year {Id}", id);
            return StatusCode(500, ApiResponse<AcademicYearResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost("{id}/set-current")]
    [Authorize(Roles = "admin,dean")]
    public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> SetCurrent(Guid id)
    {
        try
        {
            var academicYear = await _academicYearService.SetCurrentAsync(id);
            return Ok(ApiResponse<AcademicYearResponse>.SuccessResponse(academicYear, "Academic year set as current successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Academic year {Id} not found", id);
            return NotFound(ApiResponse<AcademicYearResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting academic year {Id} as current", id);
            return StatusCode(500, ApiResponse<AcademicYearResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _academicYearService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Academic year deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Academic year {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Academic year not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting academic year {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
