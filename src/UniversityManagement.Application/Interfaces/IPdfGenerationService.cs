using UniversityManagement.Shared.DTOs.Responses;

namespace UniversityManagement.Application.Interfaces;

public interface IPdfGenerationService
{
    byte[] GenerateRequestDocument(RequestResponse request);
}
