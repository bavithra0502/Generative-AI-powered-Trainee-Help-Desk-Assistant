using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    // Holds the embedded Trainee Knowledge Base chunks in memory,
    // acting as a lightweight, temporary vector store for this application.
    // Registered as a Singleton so the knowledge base survives across requests
    // once it has been built.
    public class KnowledgeBaseService
    {
        private List<EmbeddingDocument> _documents = [];
        private DateTime? _lastBuiltAtUtc;

        public void SetDocuments(List<EmbeddingDocument> documents)
        {
            _documents = documents;
            _lastBuiltAtUtc = DateTime.UtcNow;
        }

        public List<EmbeddingDocument> GetDocuments() => _documents;

        public bool IsBuilt => _documents.Count > 0;

        public DateTime? LastBuiltAtUtc => _lastBuiltAtUtc;
    }
}
