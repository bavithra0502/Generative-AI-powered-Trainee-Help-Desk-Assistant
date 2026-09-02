namespace TrainingHelpDeskApi.Models
{
    public class RetrieveDocument
    {
        // The original chunk/document information
        public EmbeddingDocument Document { get; set; } = null!;

        // How similar this document is to the trainee's question.
        // Higher value = more relevant.
        public double Similarity { get; set; }
    }
}
