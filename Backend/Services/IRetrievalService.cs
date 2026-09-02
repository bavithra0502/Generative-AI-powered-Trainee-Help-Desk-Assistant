using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public interface IRetrievalService
    {
        // Given a query embedding and the full set of knowledge base documents,
        // returns the top-K most relevant chunks by cosine similarity.
        List<RetrieveDocument> Search(float[] queryEmbedding, List<EmbeddingDocument> documents, int topK);
    }
}
