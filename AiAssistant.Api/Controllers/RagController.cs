using AiAssistant.Api.Models;
using AiAssistant.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace AiAssistant.Api.Controllers;

[ApiController]
[Route("api/rag")]
public class RagController : ControllerBase
{
    private readonly RagService _ragService;

    public RagController(RagService ragService)
    {
        _ragService = ragService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(
        [FromBody] RagRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new
            {
                error = "Question cannot be empty."
            });
        }

        var answer = await _ragService.AskAsync(
            request.Question,
            request.PatientId,
            request.Department,
            request.TopK,
            cancellationToken);

        return Ok(new
        {
            question = request.Question,
            answer
        });
    }
}