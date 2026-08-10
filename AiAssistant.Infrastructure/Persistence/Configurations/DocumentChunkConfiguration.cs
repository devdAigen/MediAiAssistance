using AiAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace AiAssistant.Infrastructure.Persistence.Configurations;

public class DocumentChunkConfiguration
    : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChunkIndex)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasColumnType("vector");

        builder.HasIndex(x => new
        {
            x.DocumentId,
            x.ChunkIndex
        })
        .IsUnique();
    }
}