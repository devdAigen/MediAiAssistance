using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiAssistant.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiAssistant.Infrastructure.LLM;

public class OpenAIService : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        var model = _configuration["OpenAI:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new InvalidOperationException(
                "OpenAI model is not configured.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/responses");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        var requestBody = new
        {
            model,
            input = prompt
        };

        var json = JsonSerializer.Serialize(requestBody);

        Console.WriteLine($"Request JSON: {json}");
        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseContent =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response JSON: {responseContent}");
            throw new HttpRequestException(
                $"OpenAI API returned " +
                $"{(int)response.StatusCode}: " +
                responseContent);
        }

        using var document =
            JsonDocument.Parse(responseContent);

        if (!document.RootElement.TryGetProperty(
                "output",
                out var output))
        {
            throw new InvalidOperationException(
                "OpenAI response did not contain output.");
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty(
                    "content",
                    out var content))
            {
                continue;
            }

            foreach (var contentItem
                in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty(
                        "text",
                        out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
            }
        }

        throw new InvalidOperationException(
            "No text was returned by OpenAI.");
    }
}