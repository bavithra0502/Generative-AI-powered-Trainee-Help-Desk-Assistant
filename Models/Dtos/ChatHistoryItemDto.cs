namespace TrainingHelpDeskApi.Models.Dtos
{
    public class ChatHistoryItemDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
