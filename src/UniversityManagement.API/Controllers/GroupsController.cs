using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GroupResponse>>>> GetAll()
    {
        try
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(ApiResponse<List<GroupResponse>>.SuccessResponse(groups));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving groups");
            return StatusCode(500, ApiResponse<List<GroupResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GroupResponse>>> GetById(Guid id)
    {
        try
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
            {
                return NotFound(ApiResponse<GroupResponse>.ErrorResponse("Group not found", statusCode: 404));
            }
            return Ok(ApiResponse<GroupResponse>.SuccessResponse(group));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving group {Id}", id);
            return StatusCode(500, ApiResponse<GroupResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpGet("series/{seriesId}")]
    public async Task<ActionResult<ApiResponse<List<GroupResponse>>>> GetBySeriesId(Guid seriesId)
    {
        try
        {
            var groups = await _groupService.GetBySeriesIdAsync(seriesId);
            return Ok(ApiResponse<List<GroupResponse>>.SuccessResponse(groups));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving groups for series {SeriesId}", seriesId);
            return StatusCode(500, ApiResponse<List<GroupResponse>>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<GroupResponse>>> Create([FromBody] CreateGroupRequest request)
    {
        try
        {
            var group = await _groupService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = group.Id },
                ApiResponse<GroupResponse>.SuccessResponse(group, "Group created successfully", 201));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Series not found when creating group");
            return NotFound(ApiResponse<GroupResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return StatusCode(500, ApiResponse<GroupResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,dean,secretariat")]
    public async Task<ActionResult<ApiResponse<GroupResponse>>> Update(Guid id, [FromBody] UpdateGroupRequest request)
    {
        try
        {
            var group = await _groupService.UpdateAsync(id, request);
            return Ok(ApiResponse<GroupResponse>.SuccessResponse(group, "Group updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found when updating group {Id}", id);
            return NotFound(ApiResponse<GroupResponse>.ErrorResponse(ex.Message, statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {Id}", id);
            return StatusCode(500, ApiResponse<GroupResponse>.ErrorResponse("An error occurred", statusCode: 500));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _groupService.DeleteAsync(id);
            return Ok(ApiResponse.SuccessResponse("Group deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Group {Id} not found", id);
            return NotFound(ApiResponse.ErrorResponse("Group not found", statusCode: 404));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {Id}", id);
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred", statusCode: 500));
        }
    }
}
