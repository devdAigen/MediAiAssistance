namespace AiAssistant.Api.Models;

public class RagRequest
{
    public string Question { get; set; } = string.Empty;

    public string? PatientId { get; set; }

    public string? Department { get; set; }

    public int TopK { get; set; } = 5;
}