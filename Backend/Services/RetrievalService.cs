using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public class RetrievalService : IRetrievalService
    {
        // Minimum similarity a chunk must have before it is considered
        // relevant enough to be included in the RAG context.
        public const double RelevanceThreshold = 0.15;

        public List<RetrieveDocument> Search(float[] queryEmbedding, List<EmbeddingDocument> documents, int topK)
        {
            if (documents == null || documents.Count == 0)
            {
                return [];
            }

            var results = new List<RetrieveDocument>();

            foreach (EmbeddingDocument document in documents)
            {
                if (document.Embedding == null || document.Embedding.Length == 0)
                {
                    continue;
                }

                double similarity = CosineSimilarity(queryEmbedding, document.Embedding);

                results.Add(new RetrieveDocument
                {
                    Document = document,
                    Similarity = similarity
                });
            }

            return results
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .ToList();
        }

        public static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("Embedding dimensions must be the same");
            }

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}
