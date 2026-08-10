namespace AiAssistant.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

     public string? Source { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<DocumentChunk> Chunks { get; set; }
        = new List<DocumentChunk>();
}