using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _programService;
    private readonly ILogger<ProgramsController> _logger;

    public ProgramsController(IProgramService programService, ILogger<ProgramsController> logger)
    {
        _programService = programService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProgramResponse>>>> GetAll()
    {
        try
        {
            var programs = await _programService.GetAllAsync();
            return Ok(ApiResponse<List<ProgramResponse>>.SuccessResponse(programs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving programs");
            return StatusCode(500, ApiResponse<List<ProgramResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProgramResponse>>> GetById(Guid id)
    {
        try
        {
            var program = await _programService.GetByIdAsync(id);
            if (program == null)
            {
                return NotFound(ApiResponse<ProgramResponse>.ErrorResponse("Program not found", statusCode: 404));
            }
            return Ok(ApiResponse<ProgramResponse>.SuccessResponse(program));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving program {Id}", id);
            return StatusCode(500, ApiResponse<ProgramResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("faculty/{facultyId}")]
    public async Task<ActionResult<ApiResponse<List<ProgramResponse>>>> GetByFacultyId(Guid facultyId)
    {
        try
        {
            var programs = await _programService.GetByFacultyIdAsync(facultyId);
            return Ok(ApiResponse<List<ProgramResponse>>.SuccessResponse(programs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving programs for faculty {FacultyId}", facultyId);
            return StatusCode(500, ApiResponse<List<ProgramResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,dean")]
    public async Task<ActionResult<ApiResponse<ProgramResponse>>> Create([FromBody] CreateProgramRequest request)
    {
        try
        {
            var program = await _programService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = program.Id },
                ApiResponse<ProgramResponse>.SuccessResponse(program, "Program created successfully", 201));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Faculty not found when creating program");
            return NotFound(ApiResponse<ProgramResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating program");
            return StatusCode(500, ApiResponse<ProgramResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,dean")]
    public async Task<ActionResult<ApiResponse<ProgramResponse>>> Update(Guid id, [FromBody] UpdateProgramRequest request)
    {
        try
        {
            var program = await _programService.UpdateAsync(id, request);
            return Ok(ApiResponse<ProgramResponse>.SuccessResponse(program, "Program updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found when updating program {Id}", id);
            return NotFound(ApiResponse<ProgramResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating program {Id}", id);
            return StatusCode(500, ApiResponse<ProgramResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _programService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Program deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Program {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Program not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting program {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
