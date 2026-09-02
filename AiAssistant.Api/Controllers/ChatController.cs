using AiAssistant.Application.Models;
using AiAssistant.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace AiAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly RagService _ragService;

    public ChatController(RagService ragService)
    {
        _ragService = ragService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(
                "Question cannot be empty.");
        }

        if (request.TopK <= 0)
        {
            return BadRequest(
                "TopK must be greater than zero.");
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