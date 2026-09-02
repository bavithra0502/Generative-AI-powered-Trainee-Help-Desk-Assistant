using Microsoft.Extensions.Options;
using OpenAI.Chat;
using TrainingHelpDeskApi.Models;

namespace TrainingHelpDeskApi.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatClient _chatClient;

        public ChatService(IOptions<OpenAISettings> options)
        {
            OpenAISettings settings = options.Value;

            _chatClient = new ChatClient(
                model: settings.ChatModel,
                apiKey: settings.ApiKey);
        }

        public async Task<string> GenerateAnswerAsync(string systemPrompt, string userPrompt)
        {
            List<ChatMessage> messages =
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            ];

            ChatCompletion response = await _chatClient.CompleteChatAsync(messages);

            return response.Content[0].Text;
        }
    }
}
