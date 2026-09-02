namespace TrainingHelpDeskApi.Models
{
    // EF Core entity used to persist trainee chat history to SQL Server.
    public class ChatLog
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? SourcesUsed { get; set; }
        public bool AnswerFoundInKnowledgeBase { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
