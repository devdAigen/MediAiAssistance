using AiAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiAssistant.Infrastructure.Persistence;

public class AiAssistantDbContext : DbContext
{
    public AiAssistantDbContext(
        DbContextOptions<AiAssistantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents =>
        Set<Document>();

    public DbSet<DocumentChunk> DocumentChunks =>
        Set<DocumentChunk>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AiAssistantDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}