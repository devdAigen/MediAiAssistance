using AiAssistant.Application.Models;
using AiAssistant.Domain.Entities;

namespace AiAssistant.Application.Interfaces;
public interface IEmbeddingService
{
    Task<EmbeddingResult> GenerateEmbeddingAsync(string text,
        string inputType,
        CancellationToken cancellationToken = default);
}
