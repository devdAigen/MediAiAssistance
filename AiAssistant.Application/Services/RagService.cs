using AiAssistant.Application.Interfaces;

namespace AiAssistant.Application.Service;

public class RagService
{
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ILanguageModel _languageModel;

    public RagService(
        IVectorSearchService vectorSearchService,
        ILanguageModel languageModel)
    {
        _vectorSearchService = vectorSearchService;
        _languageModel = languageModel;
    }

    public async Task<string> AskAsync(
        string question,
        string? patientId = null,
        string? department = null,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "Question cannot be empty.",
                nameof(question));
        }

        // 1. Retrieve relevant chunks
        var searchResults =
            await _vectorSearchService.SearchAsync(
                question,
                patientId,
                department,
                topK,
                cancellationToken);

        if (searchResults.Count == 0)
        {
            return "I could not find relevant information in the available documents.";
        }

        // 2. Build context from retrieved chunks
        var context = string.Join(
            "\n\n---\n\n",
            searchResults.Select((result, index) =>
                $"Source {index + 1} " +
                $"(similarity: {result.Score:F3}):\n" +
                result.Chunk.Content));

        Console.WriteLine($"Context built with {searchResults.Count} chunks.");
        Console.WriteLine($"Context : {context} characters.");
        // 3. Build grounded prompt
        var prompt = BuildPrompt(question, context);

Console.WriteLine($"Prompt built with {prompt} characters.");
        // 4. Ask the language model
        var answer =
            await _languageModel.GenerateAsync(
                prompt,
                cancellationToken);

        return answer;
    }

    private static string BuildPrompt(
        string question,
        string context)
    {
        return $"""
            You are a medical information assistant.

            Answer the user's question using ONLY the information
            provided in the context below.

            Do not invent facts or information that is not present
            in the context.

            If the context does not contain enough information to
            answer the question, clearly say that the available
            documents do not contain enough information.

            Do not provide a diagnosis or personalized medical
            treatment recommendation.

            Context:
            {context}

            User question:
            {question}

            Answer:
            """;
    }
}