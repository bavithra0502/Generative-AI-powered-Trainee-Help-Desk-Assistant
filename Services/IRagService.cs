using TrainingHelpDeskApi.Models.Dtos;

namespace TrainingHelpDeskApi.Services
{
    public interface IRagService
    {
        // Loads every document in the Trainee Knowledge Base, chunks it,
        // generates embeddings, and stores the result in the KnowledgeBaseService.
        Task<int> BuildKnowledgeBaseAsync();

        // Answers a trainee's question using Retrieval-Augmented Generation.
        Task<AskResponseDto> AskAsync(string question);
    }
}
