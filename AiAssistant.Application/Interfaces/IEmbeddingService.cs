using AiAssistant.Application.Models;
namespace AiAssistant.Application.Interfaces;
public interface IEmbeddingService
{
    Task<EmbeddingResult> GenerateEmbeddingAsync(string text,
        string inputType,
        CancellationToken cancellationToken = default);
}

public interface ILanguageModel
{
    Task<string> GenerateAsync(
        string question,
        IEnumerable<DocumentChunk> context);
}