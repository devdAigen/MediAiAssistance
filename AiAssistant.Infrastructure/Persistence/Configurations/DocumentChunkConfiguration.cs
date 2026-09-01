using AiAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiAssistant.Infrastructure.Persistence.Configurations;

public class DocumentChunkConfiguration
    : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(
        EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(1024)");

        builder.HasOne(x => x.Document)
            .WithMany(x => x.Chunks)
            .HasForeignKey(x => x.DocumentId);
    }
}