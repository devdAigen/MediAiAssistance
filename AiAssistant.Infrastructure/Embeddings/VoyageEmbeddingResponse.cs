using System.Text.Json.Serialization;

namespace AiAssistant.Infrastructure.Embeddings;

public sealed class VoyageEmbeddingResponse
{
    [JsonPropertyName("object")]
    public string? Object { get; init; }

    [JsonPropertyName("data")]
    public List<EmbeddingData> Data { get; init; } = [];

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("usage")]
    public EmbeddingUsage? Usage { get; init; }
}

public sealed class EmbeddingData
{
    [JsonPropertyName("object")]
    public string? Object { get; init; }

    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; init; } = [];

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public sealed class EmbeddingUsage
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}