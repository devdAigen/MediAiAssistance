using AiAssistant.Application.Interfaces;
using AiAssistant.Domain.Entities;
using AiAssistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace AiAssistant.Infrastructure.VectorStore;

public class PgVectorSearchService : IVectorSearchService
{
    private readonly AiAssistantDbContext _db;
    private readonly IEmbeddingService _embeddingService;

    public PgVectorSearchService(
        AiAssistantDbContext db,
        IEmbeddingService embeddingService)
    {
        _db = db;
        _embeddingService = embeddingService;
    }

    public async Task<List<(DocumentChunk Chunk, double Score)>> SearchAsync(
        string query,
        string? patientId = null,
        string? department = null,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Query cannot be empty.",
                nameof(query));
        }

        if (topK <= 0)
        {
            throw new ArgumentException(
                "topK must be greater than zero.",
                nameof(topK));
        }

        // 1. Generate embedding for the user's query
        var queryEmbedding =
            await _embeddingService.GenerateEmbeddingAsync(
                query,
                "document",
                cancellationToken);

        // 2. Convert float[] to pgvector Vector
        var queryVector =
            new Vector(queryEmbedding.Embedding);

        // 3. Start database query
        var candidates = _db.DocumentChunks
            .AsNoTracking()
            .AsQueryable();

        // 4. Patient filter
        if (!string.IsNullOrWhiteSpace(patientId))
        {
            candidates = candidates.Where(
                x => x.PatientId == patientId);
        }

        // 5. Department filter
        if (!string.IsNullOrWhiteSpace(department))
        {
            candidates = candidates.Where(
                x => x.Department == department);
        }

        // 6. Vector search
        //
        // NOTE:
        // x.Embedding is float[] in the Domain model.
        // Therefore we cannot call CosineDistance() directly
        // on x.Embedding.
        //
        // We will initially load the filtered chunks and calculate
        // cosine similarity in C#.
        var chunks = await candidates
            .ToListAsync(cancellationToken);

        // 7. Calculate cosine similarity
        var results = chunks
            .Select(chunk => new
            {
                Chunk = chunk,
                Score = CosineSimilarity(
                    queryEmbedding.Embedding,
                    chunk.Embedding)
            })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .ToList();

        Console.WriteLine(
            $"Vector search returned {results.Count} results.");

        return results
            .Select(x => (x.Chunk, x.Score))
            .ToList();
    }

    private static double CosineSimilarity(
        float[] a,
        float[] b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException(
                $"Vector dimensions do not match. " +
                $"Query: {a.Length}, Document: {b.Length}");
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];

            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct /
               (Math.Sqrt(magnitudeA) *
                Math.Sqrt(magnitudeB));
    }
}