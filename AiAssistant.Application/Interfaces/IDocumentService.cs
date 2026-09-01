using AiAssistant.Domain.Entities;

namespace AiAssistant.Application.Interfaces;
public interface IDocumentService
{
    Task<Document> AddDocumentAsync(Document document, CancellationToken cancellationToken = default);
}