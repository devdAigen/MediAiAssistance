using AiAssistant.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiAssistant.Infrastructure.Embeddings;
public class VoyageEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public VoyageEmbeddingService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Voyage:ApiKey"]!;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        // Call embedding API
        // Read returned vector
         return [];
    }
}