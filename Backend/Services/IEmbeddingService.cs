using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public interface IEmbeddingService
    {
        // Generates embeddings for a batch of chunked documents.
        Task<List<EmbeddingDocument>> GenerateEmbeddingAsync(List<EmbeddingDocument> documents);

        // Generates an embedding for a single piece of text (e.g. a trainee's question).
        Task<float[]> GenerateEmbeddingAsync(string text);
    }
}
