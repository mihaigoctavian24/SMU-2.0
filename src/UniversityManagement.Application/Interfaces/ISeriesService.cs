using UniversityManagement.Shared.DTOs.Requests;
using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface ISeriesService
{
    Task<SeriesResponse?> GetByIdAsync(Guid id);
    Task<List<SeriesResponse>> GetAllAsync();
    Task<List<SeriesResponse>> GetByProgramIdAsync(Guid programId);
    Task<SeriesResponse> CreateAsync(CreateSeriesRequest request);
    Task<SeriesResponse> UpdateAsync(Guid id, UpdateSeriesRequest request);
    Task DeleteAsync(Guid id);
}
