using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeriesController : ControllerBase
{
    private readonly ISeriesService _seriesService;
    private readonly ILogger<SeriesController> _logger;

    public SeriesController(ISeriesService seriesService, ILogger<SeriesController> logger)
    {
        _seriesService = seriesService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SeriesResponse>>>> GetAll()
    {
        try
        {
            var series = await _seriesService.GetAllAsync();
            return Ok(ApiResponse<List<SeriesResponse>>.SuccessResponse(series));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving series");
            return StatusCode(500, ApiResponse<List<SeriesResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SeriesResponse>>> GetById(Guid id)
    {
        try
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound(ApiResponse<SeriesResponse>.ErrorResponse("Series not found", statusCode: 404));
            }
            return Ok(ApiResponse<SeriesResponse>.SuccessResponse(series));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving series {Id}", id);
            return StatusCode(500, ApiResponse<SeriesResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("program/{programId}")]
    public async Task<ActionResult<ApiResponse<List<SeriesResponse>>>> GetByProgramId(Guid programId)
    {
        try
        {
            var series = await _seriesService.GetByProgramIdAsync(programId);
            return Ok(ApiResponse<List<SeriesResponse>>.SuccessResponse(series));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving series for program {ProgramId}", programId);
            return StatusCode(500, ApiResponse<List<SeriesResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<SeriesResponse>>> Create([FromBody] CreateSeriesRequest request)
    {
        try
        {
            var series = await _seriesService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = series.Id },
                ApiResponse<SeriesResponse>.SuccessResponse(series, "Series created successfully", 201));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Program not found when creating series");
            return NotFound(ApiResponse<SeriesResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating series");
            return StatusCode(500, ApiResponse<SeriesResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<SeriesResponse>>> Update(Guid id, [FromBody] UpdateSeriesRequest request)
    {
        try
        {
            var series = await _seriesService.UpdateAsync(id, request);
            return Ok(ApiResponse<SeriesResponse>.SuccessResponse(series, "Series updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found when updating series {Id}", id);
            return NotFound(ApiResponse<SeriesResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating series {Id}", id);
            return StatusCode(500, ApiResponse<SeriesResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _seriesService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Series deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Series {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Series not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting series {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
