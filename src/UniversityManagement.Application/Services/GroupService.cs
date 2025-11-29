using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly ISeriesRepository _seriesRepository;

    public GroupService(IGroupRepository groupRepository, ISeriesRepository seriesRepository)
    {
        _groupRepository = groupRepository;
        _seriesRepository = seriesRepository;
    }

    public async Task<GroupResponse?> GetByIdAsync(Guid id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        if (group == null) return null;

        var series = await _seriesRepository.GetByIdAsync(group.SeriesId);
        return MapToResponse(group, series?.Name ?? "Unknown");
    }

    public async Task<List<GroupResponse>> GetAllAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        var responses = new List<GroupResponse>();

        foreach (var group in groups)
        {
            var series = await _seriesRepository.GetByIdAsync(group.SeriesId);
            responses.Add(MapToResponse(group, series?.Name ?? "Unknown"));
        }

        return responses;
    }

    public async Task<List<GroupResponse>> GetBySeriesIdAsync(Guid seriesId)
    {
        var groups = await _groupRepository.GetBySeriesIdAsync(seriesId);
        var series = await _seriesRepository.GetByIdAsync(seriesId);
        var seriesName = series?.Name ?? "Unknown";

        return groups.Select(g => MapToResponse(g, seriesName)).ToList();
    }

    public async Task<GroupResponse> CreateAsync(CreateGroupRequest request)
    {
        var seriesExists = await _seriesRepository.ExistsAsync(request.SeriesId);
        if (!seriesExists)
        {
            throw new KeyNotFoundException($"Series with ID {request.SeriesId} not found");
        }

        var group = new Group
        {
            SeriesId = request.SeriesId,
            Name = request.Name
        };

        var created = await _groupRepository.CreateAsync(group);
        var series = await _seriesRepository.GetByIdAsync(created.SeriesId);
        return MapToResponse(created, series?.Name ?? "Unknown");
    }

    public async Task<GroupResponse> UpdateAsync(Guid id, UpdateGroupRequest request)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        if (group == null)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found");
        }

        var seriesExists = await _seriesRepository.ExistsAsync(request.SeriesId);
        if (!seriesExists)
        {
            throw new KeyNotFoundException($"Series with ID {request.SeriesId} not found");
        }

        group.SeriesId = request.SeriesId;
        group.Name = request.Name;

        var updated = await _groupRepository.UpdateAsync(group);
        var series = await _seriesRepository.GetByIdAsync(updated.SeriesId);
        return MapToResponse(updated, series?.Name ?? "Unknown");
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _groupRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Group with ID {id} not found");
        }

        await _groupRepository.DeleteAsync(id);
    }

    private static GroupResponse MapToResponse(Group group, string seriesName)
    {
        return new GroupResponse
        {
            Id = group.Id,
            SeriesId = group.SeriesId,
            SeriesName = seriesName,
            Name = group.Name,
            CreatedAt = group.CreatedAt,
            StudentCount = 0
        };
    }
}
