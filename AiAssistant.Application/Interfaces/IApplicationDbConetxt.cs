using AiAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiAssistant.Application.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Document> Documents {get;}
    DbSet<DocumentChunk> DocumentChunks {get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}