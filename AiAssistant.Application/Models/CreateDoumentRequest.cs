namespace AiAssistant.Application.Models;

public class RequestDocument
{
    public string FileName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public List<RequestDocumentChunk> Chunks { get; set; } = [];
}
public class RequestDocumentChunk
{
    public string Content { get; set; } = string.Empty;

    public int ChunkIndex { get; set; }

    public int? PageNumber { get; set; }

    public string? PatientId { get; set; }

    public string? Department { get; set; }

    public DateTime? VisitDate { get; set; }
}

