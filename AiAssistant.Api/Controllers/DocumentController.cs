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
    public async Task<IActionResult> Create([FromBody] RequestDocument document, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(document.FileName))
        {
            return BadRequest("Filename is required");
        }

        var documentObj = new Document
        {
            FileName = document.FileName,
            Source= document.Source,

            Chunks = document.Chunks.Select(chunk=> 
            new DocumentChunk
            {
              Content = chunk.Content,
              ChunkIndex = chunk.ChunkIndex,
              PageNumber= chunk.PageNumber,
              PatientId = chunk.PatientId,
              Department= chunk.Department,
              VisitDate = chunk.VisitDate
            }
            ).ToList()
        };

        var result = await _documentService.AddDocumentAsync(documentObj, cancellationToken);
        return Ok(result);
    }

}