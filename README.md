Project 1 (1 week)
Build a production RAG:
    • Upload PDFs 
    • Extract text 
    • Chunk 
    • Generate embeddings 
    • Store in PostgreSQL with pgvector 
    • Semantic search 
    • Answer with Claude/GPT 

Project 2 (1 week)
Build an AI chat application that:
    • Remembers users 
    • Supports conversation history 
    • Stores long-term memory 
    • Uses RAG 
    • Streams responses 

Project 3 (2 weeks)
Build an AI Agent that can:
    • Search documents 
    • Execute SQL queries 
    • Call external APIs 
    • Decide which tool to use 
    • Return structured JSON










                    ┌──────────────────────┐
                    │      Angular UI      			  │
                    └──────────┬───────────┘
                             		    │
                               		   ▼
                    ┌──────────────────────┐
                    │   ASP.NET Core API   │
                    └──────────┬───────────┘
                               		    │
                ┌─────────── ┼──────────────┐
                │              		   │              │
                ▼              		  ▼              ▼
           RAG Service      Memory        AI Service
                │              		│              │
                ▼              		▼              ▼
          PostgreSQL       PostgreSQL     Claude/GPT
          + pgvector
                │
                ▼
          Documents/Chunks



Flow

PDF ingestion
     ↓
Text extraction
     ↓
Chunking + overlap
     ↓
Embedding
     ↓
Vector storage
     ↓
Metadata filtering
     ↓
Hybrid retrieval
     ↓
Reranking
     ↓
LLM
     ↓
Streaming response
                    			AI Agent
                       			│
          ┌────────────┼────────────┐
          ▼           		 ▼            		▼
      RAG Tool      		SQL Tool     		API Tool




LEVEL1

Step 1 — Create the .NET solution

dotnet new sln -n AiAssistant

dotnet new webapi -n AiAssistant.Api

dotnet new classlib -n AiAssistant.Domain

dotnet new classlib -n AiAssistant.Application

dotnet new classlib -n AiAssistant.Infrastructure

dotnet sln add AiAssistant.Api
dotnet sln add AiAssistant.Domain
dotnet sln add AiAssistant.Application
dotnet sln add AiAssistant.Infrastructure


PROJECT LAYOUT

AiAssistant.Api
       │
       ▼
AiAssistant.Application
       │
       ▼
AiAssistant.Domain

AiAssistant.Infrastructure
       │
       ├── PostgreSQL
       ├── pgvector
       ├── Embedding provider
       └── LLM provider
step2 -
model to save the details as vector metadata
DocumentChunk
│
├── Content
├── PageNumber
├── ChunkIndex
│
├── PatientId
├── Department
├── VisitDate
│
└── Embedding


Step 3 – Validate and debug the flow for correct chunks

Question
   ↓
Embedding
   ↓
Vector Search
   ↓
Correct chunks?


Step 4: retrieval

Document
   ↓
Chunk
   ↓
Embedding
   ↓
Vector


Question
   ↓
Embedding
   ↓
Compare with stored vectors
   ↓
Similarity score
   ↓
Top K chunks

Vector DB
    │
    ├── stores vectors
    ├── stores metadata
    └── performs similarity search
step 5: metdata filtering


Chunk 1
PatientId = P100
Department = Cardiology

Chunk 2
PatientId = P200
Department = Oncology

Chunk 3
PatientId = P100
Department = Cardiology

10 Million records



10 million chunks
       │
       ▼
PatientId = P100
       │
       ▼
Department = Cardiology
       │
       ▼
Candidate chunks
       │
       ▼
Vector similarity
       │
       ▼
Top 5(Top K and Abpve threshhold)




Embedding
It is representations of text with similar semantic information that tend to be close.
Core Rag Architecture
                   USER QUESTION
                         │
                         ▼
                ┌─────────────────┐
                │ Query Processing│
                └────────┬────────┘
                         │
              ┌──────────┴──────────┐
              │                     │
              ▼                     ▼
        Metadata Filter       Query Embedding
              │                     │
              └──────────┬──────────┘
                         ▼
                  Vector Search
                         │
                         ▼
                     Top K Chunks
                         │
                         ▼
                    ┌─────────┐
                    │ Claude  │
                    │ / GPT   │
                    └────┬────┘
                         │
                         ▼
                       Answer
text → embedding → vector → similarity → retrieval


                    Question
                       │
                       ▼
             IEmbeddingService
                       │
                       ▼
                Query Vector
                       │
                       ▼
              VectorSearchService
                       │
             ┌─────────┴─────────┐
             │                   │
       Metadata Filter       Similarity
             │                   │
             └─────────┬─────────┘
                       ▼
                   Top K
                       │
                       ▼
              DocumentChunk[]
Level 2 : embedding real embedding model voyage
dotnet user-secrets init

dotnet user-secrets set "Voyage:ApiKey" "YOUR_API_KEY"\
configuration["Voyage:ApiKey"]

Embedding
It is representations of text with similar semantic information that tend to be close.


RoadMap
1. Real embedding API              ← NOW
       ↓
2. Inspect similarity scores
       ↓
3. PostgreSQL
       ↓
4. pgvector
       ↓
5. Metadata filtering
       ↓
6. Chunking + overlapping chunks
       ↓
7. PDF ingestion + page numbers
       ↓
8. Hybrid retrieval
       ↓
9. Reranking
       ↓
10. Claude integration
       ↓
11. Conversation memory
       ↓
12. Streaming
       ↓
13. Tool/function calling
       ↓
14. Agent
       ↓
15. Evaluation
       ↓
16. Docker
       ↓
17. AWS deployment
Developed Project structure
AiAssistant/
│
├── AiAssistant.sln
│
├── src/
│   │
│   ├── AiAssistant.Api/
│   │   │
│   │   ├── Controllers/
│   │   │   └── ChatController.cs
│   │   │
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── AiAssistant.Api.csproj
│   │
│   ├── AiAssistant.Domain/
│   │   │
│   │   ├── Entities/
│   │   │   ├── Document.cs
│   │   │   └── DocumentChunk.cs
│   │   │
│   │   └── AiAssistant.Domain.csproj
│   │
│   ├── AiAssistant.Application/
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── IEmbeddingService.cs
│   │   │   ├── ILanguageModel.cs
│   │   │   ├── IDocumentService.cs
│   │   │   └── IVectorSearchService.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── DocumentService.cs
│   │   │   ├── VectorSearchService.cs
│   │   │   └── RagService.cs
│   │   │
│   │   └── AiAssistant.Application.csproj
│   │
│   └── AiAssistant.Infrastructure/
│       │
│       ├── Embeddings/
│       │   ├── VoyageEmbeddingService.cs
│       │   └── SimpleEmbeddingService.cs
│       │
│       ├── LLM/
│       │   ├── ClaudeService.cs
│       │   └── OpenAIService.cs
│       │
│       ├── Persistence/
│       │   ├── AiAssistantDbContext.cs
│       │   └── Configurations/
│       │       ├── DocumentConfiguration.cs
│       │       └── DocumentChunkConfiguration.cs
│       │
│       ├── VectorStore/
│       │   └── PgVectorSearchService.cs
│       │
│       └── AiAssistant.Infrastructure.csproj
│
├── tests/
│   │
│   ├── AiAssistant.Application.Tests/
│   │   ├── VectorSearchServiceTests.cs
│   │   ├── RagServiceTests.cs
│   │   └── AiAssistant.Application.Tests.csproj
│   │
│   └── AiAssistant.Infrastructure.Tests/
│       ├── EmbeddingServiceTests.cs
│       └── AiAssistant.Infrastructure.Tests.csproj
│
└── README.md



mkdir -p AiAssistant.Api/Controllers

mkdir -p AiAssistant.Domain/Entities

mkdir -p AiAssistant.Application/Interfaces
mkdir -p AiAssistant.Application/Services

mkdir -p AiAssistant.Infrastructure/Embeddings
mkdir -p AiAssistant.Infrastructure/LLM
mkdir -p AiAssistant.Infrastructure/Persistence/Configurations
mkdir -p AiAssistant.Infrastructure/VectorStore

mkdir -p tests/AiAssistant.Application.Tests
mkdir -p tests/AiAssistant.Infrastructure.Tests

touch AiAssistant.Application/Interfaces/ILanguageModel.cs
touch AiAssistant.Application/Interfaces/IDocumentService.cs
touch AiAssistant.Application/Interfaces/IvectorSearchService.cs

touch AiAssistant.Application/Services/DocumentService.cs
touch AiAssistant.Application/Services/VectorSearchService.cs
touch AiAssistant.Application/Services/RagService.cs
touch AiAssistant.Api/Controllers/ChatController.cs

touch AiAssistant.Infrastructure/Embeddings/VoyageEmbeddingService.cs
touch AiAssistant.Infrastructure/Embeddings/SimpleEmbeddingService.cs

touch AiAssistant.Infrastructure/LLM/ClaudeService.cs
touch AiAssistant.Infrastructure/LLM/OpenAIService.cs

touch AiAssistant.Infrastructure/Persistence/AiAssistantDbContext.cs

touch AiAssistant.Infrastructure/Persistence/Configurations/DocumentConfiguration.cs
touch AiAssistant.Infrastructure/Persistence/Configurations/DocumentChunkConfiguration.cs

touch AiAssistant.Infrastructure/VectorStore/PgVectorSearchService.cs
