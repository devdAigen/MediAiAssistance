using AiAssistant.Domain.Entities;

namespace AiAssistant.Application.Interfaces;

public interface IVectorSearchService
{
    Task<List<(DocumentChunk Chunk, double Score)>> SearchAsync(
        string query,
        string? patientId = null,
        string? department = null,
        int topK = 5,
        CancellationToken cancellationToken = default);
}