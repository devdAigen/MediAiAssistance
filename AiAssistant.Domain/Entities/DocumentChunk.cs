public class DocumentChunk
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public string Content { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public int? PageNumber { get; set; }

    public string? PatientId { get; set; }

    public string? Department { get; set; }

    public DateTime? VisitDate { get; set; }
    public DateTime? CreatedAt { get; set; }

    public float[] Embedding { get; set; } = [];
}