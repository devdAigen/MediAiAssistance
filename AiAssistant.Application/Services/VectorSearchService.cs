using AiAssistant.Application.Interfaces;
namespace AiAssistant.Application.Service;
public class VectorSearchService
{
   public List<(DocumentChunk Chunk, double Score)> Search(
    string query,
    List<DocumentChunk> chunks,
    IEmbeddingService embeddingService,
    string? patientId = null,
    string? department = null,
    int topK = 3)
{
    var candidates = chunks.AsEnumerable();

    if (!string.IsNullOrEmpty(patientId))
    {
        candidates = candidates.Where(
            x => x.PatientId == patientId);
    }

    if (!string.IsNullOrEmpty(department))
    {
        candidates = candidates.Where(
            x => x.Department == department);
    }

    var queryVector = embeddingService
        .GenerateEmbeddingAsync(query)
        .GetAwaiter()
        .GetResult();

    return candidates
        .Select(chunk => new
        {
            Chunk = chunk,
            Score = CosineSimilarity(
                queryVector,
                chunk.Embedding)
        })
        .OrderByDescending(x => x.Score)
        .Take(topK)
        .Select(x => (x.Chunk, x.Score))
        .ToList();
}

    private static double CosineSimilarity(
        float[] a,
        float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vector dimensions must match.");

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
            return 0;

        return dotProduct /
               (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}