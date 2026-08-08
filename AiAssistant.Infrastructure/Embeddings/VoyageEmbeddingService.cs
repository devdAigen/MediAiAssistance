using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AiAssistant.Application.Interfaces;
using AiAssistant.Application.Models;
using Microsoft.Extensions.Configuration;

namespace AiAssistant.Infrastructure.Embeddings;

public class VoyageEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    private const string Model = "voyage-4-lite";

    public VoyageEmbeddingService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["VoyageApi:ApiKey"]
            ?? throw new InvalidOperationException(
                "VoyageApi:ApiKey is not configured.");

        _baseUrl = configuration["VoyageApi:BaseUrlMongoDB"]
            ?? throw new InvalidOperationException(
                "VoyageApiMongoDB:BaseUrlMongoDB is not configured.");
    }

    public async Task<EmbeddingResult> GenerateEmbeddingAsync(
        string text,
        string inputType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be null or whitespace.",
                nameof(text));
        }

        if (string.IsNullOrWhiteSpace(inputType))
        {
            throw new ArgumentException(
                "Input type cannot be empty.",
                nameof(inputType));
        }

        var request = new VoyageEmbeddingRequest
        {
            Input = text,
            Model = Model,
            InputType = inputType
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_baseUrl}/v1/embeddings");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        var responseBody =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Embedding API returned {(int)response.StatusCode} " +
                $"{response.StatusCode}: {responseBody}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<VoyageEmbeddingResponse>(
                cancellationToken);

        if (result?.Data == null || result.Data.Count == 0)
        {
            throw new InvalidOperationException(
                $"No embedding returned. Response: {responseBody}");
        }

        var data = result.Data[0];

        return new EmbeddingResult
        {
            Embedding = data.Embedding.ToArray(),
            Model = result.Model,
            TotalTokens = result.Usage?.TotalTokens ?? 0,
            Index = data.Index,
            Text = data.Text
        };
    }

    private sealed class VoyageEmbeddingRequest
    {
        [JsonPropertyName("input")]
        public required string Input { get; init; }

        [JsonPropertyName("model")]
        public required string Model { get; init; }

        [JsonPropertyName("input_type")]
        public required string InputType { get; init; }
    }
}