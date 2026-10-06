using AiAssistant.Application.Interfaces;

namespace AiAssistant.Application.Service;

public class RagService
{
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ILanguageModel _languageModel;

    // Initial threshold.
    // We will tune this using your actual search results.
    private const double MinimumSimilarityScore = 0.75;

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

        Console.WriteLine(
            $"Vector search returned {searchResults.Count} results.");

        // 2. Remove low-relevance results
        var relevantResults = searchResults
         //   .Where(result => result.Score >= MinimumSimilarityScore)
            .ToList();

        Console.WriteLine(
            $"Results above similarity threshold " +
            $"({MinimumSimilarityScore:F2}): {relevantResults.Count}");

        // 3. No sufficiently relevant context
        // if (relevantResults.Count == 0)
        // {
        //     return "I could not find sufficiently relevant information in the available documents.";
        // }

        foreach (var result in searchResults)
        {
            Console.WriteLine(
                $"Source: {result.Chunk}, " +
                $"Similarity: {result.Score:F3}");
        }

        // 4. Build context from relevant chunks
        var context = string.Join(
            "\n\n---\n\n",
            relevantResults.Select((result, index) =>
                $"Source {index + 1} " +
                $"(similarity: {result.Score:F3}):\n" +
                result.Chunk.Content));

        Console.WriteLine(
            $"Context built with {relevantResults.Count} chunks.");

        Console.WriteLine(
            $"Context : {context} characters.");

        // 5. Build grounded prompt
        var prompt = BuildPrompt(
            question,
            context);

        Console.WriteLine(
            $"Prompt length: {prompt.Length} characters.");

        // 6. Ask Gemini through ILanguageModel
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

            Your task is to answer the user's question using ONLY
            the information contained in the provided document context.

            IMPORTANT RULES:

            1. Do not invent facts.
            2. Do not use outside knowledge.
            3. Do not make assumptions about the patient.
            4. If the context does not contain enough information,
               say that the available documents do not contain
               enough information to answer the question.
            5. Do not provide a diagnosis.
            6. Do not provide personalized medical treatment
               recommendations.
            7. Keep the answer clear and concise.
            8. When possible, mention which source supports the answer.

            DOCUMENT CONTEXT:
            {context}

            USER QUESTION:
            {question}

            ANSWER:
            """;
    }
}