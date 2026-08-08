namespace AiAssistant.Application.Models;

public class EmbeddingResult
{
    public required float[] Embedding { get; init; }

    public string? Model { get; init; }

    public int TotalTokens { get; init; }

    public int Index { get; init; }

    public string? Text { get; init; }
}