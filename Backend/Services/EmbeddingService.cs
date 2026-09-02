using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(IOptions<OpenAISettings> options)
        {
            OpenAISettings settings = options.Value;

            _embeddingClient = new EmbeddingClient(
                model: settings.EmbeddingModel,
                apiKey: settings.ApiKey);
        }

        public async Task<List<EmbeddingDocument>> GenerateEmbeddingAsync(List<EmbeddingDocument> documents)
        {
            foreach (EmbeddingDocument document in documents)
            {
                document.Embedding = await GenerateEmbeddingAsync(document.Text);
            }

            return documents;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be empty", nameof(text));
            }

            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
            return embedding.ToFloats().ToArray();
        }
    }
}
