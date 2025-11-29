using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Services;

public class SeriesService : ISeriesService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly IProgramRepository _programRepository;

    public SeriesService(ISeriesRepository seriesRepository, IProgramRepository programRepository)
    {
        _seriesRepository = seriesRepository;
        _programRepository = programRepository;
    }

    public async Task<SeriesResponse?> GetByIdAsync(Guid id)
    {
        var series = await _seriesRepository.GetByIdAsync(id);
        if (series == null) return null;

        var program = await _programRepository.GetByIdAsync(series.ProgramId);
        return MapToResponse(series, program?.Name ?? "Unknown");
    }

    public async Task<List<SeriesResponse>> GetAllAsync()
    {
        var seriesList = await _seriesRepository.GetAllAsync();
        var responses = new List<SeriesResponse>();

        foreach (var series in seriesList)
        {
            var program = await _programRepository.GetByIdAsync(series.ProgramId);
            responses.Add(MapToResponse(series, program?.Name ?? "Unknown"));
        }

        return responses;
    }

    public async Task<List<SeriesResponse>> GetByProgramIdAsync(Guid programId)
    {
        var seriesList = await _seriesRepository.GetByProgramIdAsync(programId);
        var program = await _programRepository.GetByIdAsync(programId);
        var programName = program?.Name ?? "Unknown";

        return seriesList.Select(s => MapToResponse(s, programName)).ToList();
    }

    public async Task<SeriesResponse> CreateAsync(CreateSeriesRequest request)
    {
        var programExists = await _programRepository.ExistsAsync(request.ProgramId);
        if (!programExists)
        {
            throw new KeyNotFoundException($"Program with ID {request.ProgramId} not found");
        }

        var series = new Series
        {
            ProgramId = request.ProgramId,
            Name = request.Name,
            YearOfStudy = request.YearOfStudy
        };

        var created = await _seriesRepository.CreateAsync(series);
        var program = await _programRepository.GetByIdAsync(created.ProgramId);
        return MapToResponse(created, program?.Name ?? "Unknown");
    }

    public async Task<SeriesResponse> UpdateAsync(Guid id, UpdateSeriesRequest request)
    {
        var series = await _seriesRepository.GetByIdAsync(id);
        if (series == null)
        {
            throw new KeyNotFoundException($"Series with ID {id} not found");
        }

        var programExists = await _programRepository.ExistsAsync(request.ProgramId);
        if (!programExists)
        {
            throw new KeyNotFoundException($"Program with ID {request.ProgramId} not found");
        }

        series.ProgramId = request.ProgramId;
        series.Name = request.Name;
        series.YearOfStudy = request.YearOfStudy;

        var updated = await _seriesRepository.UpdateAsync(series);
        var program = await _programRepository.GetByIdAsync(updated.ProgramId);
        return MapToResponse(updated, program?.Name ?? "Unknown");
    }

    public async Task DeleteAsync(Guid id)
    {
        var exists = await _seriesRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Series with ID {id} not found");
        }

        await _seriesRepository.DeleteAsync(id);
    }

    private static SeriesResponse MapToResponse(Series series, string programName)
    {
        return new SeriesResponse
        {
            Id = series.Id,
            ProgramId = series.ProgramId,
            ProgramName = programName,
            Name = series.Name,
            YearOfStudy = series.YearOfStudy,
            CreatedAt = series.CreatedAt,
            GroupCount = 0
        };
    }
}
