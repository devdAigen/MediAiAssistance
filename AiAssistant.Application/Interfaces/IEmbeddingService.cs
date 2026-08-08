namespace AiAssistant.Application.Interfaces;
public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}

public interface ILanguageModel
{
    Task<string> GenerateAsync(
        string question,
        IEnumerable<DocumentChunk> context);
}