using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public class ChunkingService : IChunkingService
    {
        public List<EmbeddingDocument> CreateChunks(string text, string source)
        {
            // Split the document into individual, non-empty lines.
            // Each line in our policy documents represents one self-contained
            // statement, which works well as a retrieval unit for this knowledge base.
            var lines = text
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var documents = new List<EmbeddingDocument>();

            for (int i = 0; i < lines.Count; i++)
            {
                documents.Add(new EmbeddingDocument
                {
                    Id = $"{Path.GetFileNameWithoutExtension(source)}-{i + 1}",
                    Text = lines[i],
                    Source = source
                    // Embedding is populated later by the EmbeddingService.
                });
            }

            return documents;
        }
    }
}
