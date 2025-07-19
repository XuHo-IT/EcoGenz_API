using Application.Entities.Base.AIChat;
using EcoGreen.Helpers;

namespace EcoGreen.Service.Chat
{
    public class AIChatService
    {
        private readonly string _geminiApiKey;

        public AIChatService(string geminiApiKey = null)
        {
            _geminiApiKey = geminiApiKey;
        }

        public async Task<string> ChatAsync(ChatModel model, string userMessage)
        {
            // Nếu có Gemini API key, sử dụng Gemini
            if (!string.IsNullOrEmpty(_geminiApiKey))
            {
                return await ChatWithGeminiAsync(model, userMessage);
            }
            
            // Ngược lại sử dụng Ollama
            return await ChatWithOllamaAsync(model, userMessage);
        }

        private async Task<string> ChatWithGeminiAsync(ChatModel model, string userMessage)
        {
            var geminiClient = new GeminiClient(_geminiApiKey);
            
            // Thêm tin nhắn người dùng vào lịch sử
            model.Messages.Add(new Message { Role = "user", Content = userMessage });

            // Gửi toàn bộ lịch sử chat đến Gemini
            var reply = await geminiClient.AskWithHistoryAsync(model.Messages);

            // Thêm phản hồi của AI vào lịch sử
            model.Messages.Add(new Message { Role = "assistant", Content = reply });

            return reply;
        }

        private async Task<string> ChatWithOllamaAsync(ChatModel model, string userMessage)
        {
            using var ollama = new AIChatAPIClient();

            model.Messages.Add(new Message { Role = "user", Content = userMessage });

            var result = await ollama.GenerateChatAsync(model);

            model.Messages.Add(new Message { Role = "assistant", Content = result.Message.Content });

            return result.Message.Content;
        }


    }
}
