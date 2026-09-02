using AiAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IVectorSearchService _vectorSearchService;

    public SearchController(
        IVectorSearchService vectorSearchService)
    {
        _vectorSearchService = vectorSearchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] string? patientId = null,
        [FromQuery] string? department = null,
        [FromQuery] int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query cannot be empty.");
        }

        var results = await _vectorSearchService.SearchAsync(
            query,
            patientId,
            department,
            topK,
            cancellationToken);

        var response = results.Select(x => new
        {
            content = x.Chunk.Content,
            score = x.Score,
            chunkIndex = x.Chunk.ChunkIndex,
            pageNumber = x.Chunk.PageNumber,
            patientId = x.Chunk.PatientId,
            department = x.Chunk.Department,
            visitDate = x.Chunk.VisitDate
        });

        return Ok(response);
    }
}