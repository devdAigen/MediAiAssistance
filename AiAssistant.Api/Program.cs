using Microsoft.EntityFrameworkCore;
using AiAssistant.Infrastructure.Persistence;
using AiAssistant.Infrastructure.Embeddings;
using AiAssistant.Application.Interfaces;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using AiAssistant.Infrastructure.VectorStore;
using AiAssistant.Application.Service;
using AiAssistant.Infrastructure.LLM;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IEmbeddingService, VoyageEmbeddingService>();
builder.Services.AddDbContext<AiAssistantDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AiAssistantDb"),
        npgsqlOptions =>
        {
            npgsqlOptions.UseVector();
        });
});

builder.Services.AddScoped<IApplicationDbContext>(s=> s.GetRequiredService<AiAssistantDbContext>());
builder.Services.AddScoped<IDocumentService,DocumentService>();
builder.Services.AddScoped<IVectorSearchService,PgVectorSearchService>();
builder.Services.AddScoped<RagService>();
builder.Services.AddScoped<ILanguageModel,GeminiService>();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
