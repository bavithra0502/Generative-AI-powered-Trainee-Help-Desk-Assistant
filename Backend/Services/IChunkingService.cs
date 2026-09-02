using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public interface IChunkingService
    {
        // Splits raw document text into smaller, retrievable chunks.
        List<EmbeddingDocument> CreateChunks(string text, string source);
    }
}
