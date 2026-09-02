namespace TrainingHelpDeskApi.Services
{
    public interface IChatService
    {
        Task<string> GenerateAnswerAsync(string systemPrompt, string userPrompt);
    }
}
