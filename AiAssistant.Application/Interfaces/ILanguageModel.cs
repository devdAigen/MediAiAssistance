namespace AiAssistant.Application.Interfaces;

public interface ILanguageModel
{
    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}