using AiAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiAssistant.Api.Controllers;

[ApiController]
[Route("api/gemini")]
public class GeminiController : ControllerBase
{
    private readonly ILanguageModel _languageModel;

    public GeminiController(ILanguageModel languageModel)
    {
        _languageModel = languageModel;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test(
        CancellationToken cancellationToken)
    {
        var prompt = """
                     Explain Retrieval Augmented Generation (RAG)
                     in simple terms for a .NET developer.
                     Keep the answer under 150 words.
                     """;

        var response = await _languageModel.GenerateAsync(
            prompt,
            cancellationToken);

        return Ok(new
        {
            response
        });
    }
}