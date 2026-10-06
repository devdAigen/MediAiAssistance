using System.Text;
using System.Text.Json;
using AiAssistant.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiAssistant.Infrastructure.LLM;

public class GeminiService : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException(
                "Gemini:ApiKey is not configured.");

        _model = configuration["Gemini:Model"]
            ?? "gemini-2.5-flash";
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent";

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        using var httpRequest =
            new HttpRequestMessage(HttpMethod.Post, url);

        httpRequest.Headers.Add("x-goog-api-key", _apiKey);

        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        var responseContent =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Gemini API request failed. " +
                $"Status: {(int)response.StatusCode} " +
                $"({response.StatusCode}). " +
                $"Response: {responseContent}");
        }

        using var json =
            JsonDocument.Parse(responseContent);

        var text =
            json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

        return text ?? string.Empty;
    }
}