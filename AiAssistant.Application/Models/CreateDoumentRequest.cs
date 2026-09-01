namespace AiAssistant.Application.Models;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public List<DocumentChunkResponse> Chunks { get; set; } = [];
}

public class DocumentChunkResponse
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int? PageNumber { get; set; }
    public string? PatientId { get; set; }
    public string? Department { get; set; }
    public DateTime? VisitDate { get; set; }

    // Don't return the embedding itself through the API.
    public bool HasEmbedding { get; set; }
}