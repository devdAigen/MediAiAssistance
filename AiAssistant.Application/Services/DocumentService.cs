
using AiAssistant.Application.Interfaces;
using AiAssistant.Domain.Entities;
public class DocumentService : IDocumentService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IApplicationDbContext _applicationDbContext;

    public DocumentService(IApplicationDbContext applicationDbContext, IEmbeddingService embeddingService)
    {
        _applicationDbContext = applicationDbContext;
        _embeddingService = embeddingService;
    }

    public async Task<Document> AddDocumentAsync(Document document, CancellationToken cancellationToken = default)
    {
        foreach (var chunk in document.Chunks)
        {
            var result = await _embeddingService.GenerateEmbeddingAsync(chunk.Content, "document", cancellationToken);
            chunk.CreatedAt = DateTime.UtcNow;
            chunk.UpdatedAt = DateTime.UtcNow;
             chunk.Embedding = result.Embedding;
            chunk.DocumentId = document.Id;
            Console.WriteLine(result.Embedding.Length);

        }

        document.CreatedAtUtc = DateTime.UtcNow;
        document.UpdatedAtUtc = DateTime.UtcNow;
        _applicationDbContext.Documents.Add(document);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return document;
    }


}

