using AiAssistant.Application.Interfaces;
using AiAssistant.Application.Service;
using AiAssistant.Application.Models;
using AiAssistant.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AIAssstant.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EmbeddingController : ControllerBase
{
    private readonly IEmbeddingService _embeddingService;
    public EmbeddingController(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    [HttpGet("generate")]
    public async Task<IActionResult> GetEmbedding(string text){
        if(string.IsNullOrWhiteSpace(text)){
            return BadRequest("Text cannot be null or whitespace.");
        }

        try
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(text,
        "document",CancellationToken.None);
          return Ok(embedding);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }   
          
    }

    [HttpGet("generate-batch")]
    public async Task<IActionResult> GetBatchEmbeddings([FromQuery] List<string> texts)
    {
        if (texts == null || texts.Count == 0)
        {
            return BadRequest("Texts cannot be null or empty.");
        }


        try
        {
            var embeddings = new List<EmbeddingResult>();
            foreach (var text in texts)
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(text,
                    "document", CancellationToken.None);
                embeddings.Add(embedding);
            }
          //  int topK = 3; // You can adjust this value as needed
            int embeddingDimension = embeddings.FirstOrDefault()?.Embedding.Length ?? 0;
            int numEmbeddings = embeddings.Count;

            for(int i=0;i<numEmbeddings;i++)
            {
                var embedding = embeddings[i];
                if (embedding.Embedding == null || embedding.Embedding.Length == 0)
                {
                    return StatusCode(500, $"Failed to generate embedding for text at index {i}.");
                }
                if(i==numEmbeddings-1)
                {
                    break;
                }
                var result = VectorSearchService.CosineSimilarity(embedding.Embedding, embeddings[i+1].Embedding);
                Console.WriteLine($"Cosine similarity between embedding {i} and embedding {i+1}: {result}");
            }
            
            return Ok(embeddings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }    
}