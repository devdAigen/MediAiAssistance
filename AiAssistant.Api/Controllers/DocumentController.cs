using AiAssistant.Application.Interfaces;
using AiAssistant.Application.Models;
using AiAssistant.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AIAssstant.Api.Controllers;

[ApiController]
[Route("api/controller")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentResponse document, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(document.FileName))
        {
            return BadRequest("Filename is required");
        }

        var documentObj = new Document
        {
            FileName = document.FileName,
            Source = document.Source,

            Chunks = document.Chunks.Select(chunk =>
            new DocumentChunk
            {
                Content = chunk.Content,
                ChunkIndex = chunk.ChunkIndex,
                PageNumber = chunk.PageNumber,
                PatientId = chunk.PatientId,
                Department = chunk.Department,
                VisitDate = chunk.VisitDate
            }
            ).ToList()
        };

        var result = await _documentService.AddDocumentAsync(documentObj, cancellationToken);

        var response = new DocumentResponse
        {
            Id = result.Id,
            FileName = result.FileName,
            Source = result.Source,
            Created = result.CreatedAtUtc,
            Updated = result.UpdatedAtUtc,

            Chunks = result.Chunks.Select(chunk => new DocumentChunkResponse
            {
                Id = chunk.Id,
                DocumentId = chunk.DocumentId,
                Content = chunk.Content,
                ChunkIndex = chunk.ChunkIndex,
                PageNumber = chunk.PageNumber,
                PatientId = chunk.PatientId,
                Department = chunk.Department,
                VisitDate = chunk.VisitDate,
                HasEmbedding = chunk.Embedding is { Length: > 0 }
            }).ToList()
        };

        return Ok(response);
    }

}