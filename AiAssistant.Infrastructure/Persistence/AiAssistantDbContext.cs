using AiAssistant.Application.Interfaces;
using AiAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace AiAssistant.Infrastructure.Persistence;

public class AiAssistantDbContext : DbContext,IApplicationDbContext
{
    public AiAssistantDbContext(
        DbContextOptions<AiAssistantDbContext> options)
        : base(options)
    {
    }

    // public DbSet<Document> Documents =>
    //     Set<Document>();

    // public DbSet<DocumentChunk> DocumentChunks =>
    //     Set<DocumentChunk>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AiAssistantDbContext).Assembly);
           modelBuilder.Entity<DocumentChunk>()
           .Property(x=>x.Embedding)
           .HasConversion(
            value=> new Vector(value),
            value=>value.ToArray())
            .HasColumnType("vector(1024)");

            

        base.OnModelCreating(modelBuilder);
    }
}