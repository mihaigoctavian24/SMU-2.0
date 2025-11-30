using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

/// <summary>
/// Controller for managing professors
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfessorsController : ControllerBase
{
    private readonly IProfessorService _professorService;
    private readonly ILogger<ProfessorsController> _logger;

    public ProfessorsController(
        IProfessorService professorService,
        ILogger<ProfessorsController> logger)
    {
        _professorService = professorService;
        _logger = logger;
    }

    /// <summary>
    /// Get all professors
    /// </summary>
    /// <returns>List of professors</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProfessorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProfessorResponse>>>> GetAll()
    {
        var professors = await _professorService.GetAllAsync();
        return Ok(ApiResponse<List<ProfessorResponse>>.SuccessResponse(professors));
    }

    /// <summary>
    /// Get professor by ID
    /// </summary>
    /// <param name="id">Professor ID</param>
    /// <returns>Professor details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfessorResponse>>> GetById(Guid id)
    {
        var professor = await _professorService.GetByIdAsync(id);
        if (professor == null)
        {
            return NotFound(ApiResponse<ProfessorResponse>.ErrorResponse("Professor not found", 404));
        }

        return Ok(ApiResponse<ProfessorResponse>.SuccessResponse(professor));
    }

    /// <summary>
    /// Get professor by User ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Professor details</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfessorResponse>>> GetByUserId(Guid userId)
    {
        var professor = await _professorService.GetByUserIdAsync(userId);
        if (professor == null)
        {
            return NotFound(ApiResponse<ProfessorResponse>.ErrorResponse("Professor not found for this user", 404));
        }

        return Ok(ApiResponse<ProfessorResponse>.SuccessResponse(professor));
    }

    /// <summary>
    /// Get professors by department
    /// </summary>
    /// <param name="department">Department name</param>
    /// <returns>List of professors in the department</returns>
    [HttpGet("department/{department}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProfessorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProfessorResponse>>>> GetByDepartment(string department)
    {
        var professors = await _professorService.GetByDepartmentAsync(department);
        return Ok(ApiResponse<List<ProfessorResponse>>.SuccessResponse(professors));
    }

    /// <summary>
    /// Get professors by faculty
    /// </summary>
    /// <param name="facultyId">Faculty ID</param>
    /// <returns>List of professors in the faculty</returns>
    [HttpGet("faculty/{facultyId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProfessorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProfessorResponse>>>> GetByFaculty(Guid facultyId)
    {
        var professors = await _professorService.GetByFacultyAsync(facultyId);
        return Ok(ApiResponse<List<ProfessorResponse>>.SuccessResponse(professors));
    }

    /// <summary>
    /// Search professors
    /// </summary>
    /// <param name="searchTerm">Search term (name, title, department)</param>
    /// <returns>List of matching professors</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<ProfessorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProfessorResponse>>>> Search([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest(ApiResponse<List<ProfessorResponse>>.ErrorResponse("Search term is required", 400));
        }

        var professors = await _professorService.SearchAsync(searchTerm);
        return Ok(ApiResponse<List<ProfessorResponse>>.SuccessResponse(professors));
    }

    /// <summary>
    /// Create a new professor
    /// </summary>
    /// <param name="request">Professor creation data</param>
    /// <returns>Created professor</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Secretary")]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProfessorResponse>>> Create([FromBody] CreateProfessorRequest request)
    {
        try
        {
            var professor = await _professorService.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = professor.Id },
                ApiResponse<ProfessorResponse>.SuccessResponse(professor, "Professor created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProfessorResponse>.ErrorResponse(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProfessorResponse>.ErrorResponse(ex.Message, 400));
        }
    }

    /// <summary>
    /// Update an existing professor
    /// </summary>
    /// <param name="id">Professor ID</param>
    /// <param name="request">Professor update data</param>
    /// <returns>Updated professor</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Secretary,Professor")]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProfessorResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProfessorResponse>>> Update(Guid id, [FromBody] UpdateProfessorRequest request)
    {
        try
        {
            var professor = await _professorService.UpdateAsync(id, request);
            return Ok(ApiResponse<ProfessorResponse>.SuccessResponse(professor, "Professor updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ProfessorResponse>.ErrorResponse(ex.Message, 404));
        }
    }

    /// <summary>
    /// Delete a professor
    /// </summary>
    /// <param name="id">Professor ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _professorService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Professor deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, 404));
        }
    }
}
