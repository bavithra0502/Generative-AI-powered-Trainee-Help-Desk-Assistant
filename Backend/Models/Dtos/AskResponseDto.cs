namespace TrainingHelpDeskApi.Models.Dtos
{
    public class AskResponseDto
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = [];
        public bool AnswerFoundInKnowledgeBase { get; set; }
    }
}
