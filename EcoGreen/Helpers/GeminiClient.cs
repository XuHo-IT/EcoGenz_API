using System.Text.Json;
using System.Text;
using Application.Entities.Base.AIChat;

namespace EcoGreen.Helpers
{
    public class GeminiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<string> AskAsync(string userInput)
        {
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = userInput }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {jsonResponse}");

            using var doc = JsonDocument.Parse(jsonResponse);
            var reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return reply ?? "[Empty reply]";
        }

        public async Task<string> AskWithHistoryAsync(List<Message> messages)
        {
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            // Tách system prompt và conversation messages
            var systemPrompt = "";
            var conversationMessages = new List<Message>();
            
            foreach (var msg in messages)
            {
                if (msg.Role == "system")
                {
                    systemPrompt = msg.Content;
                }
                else
                {
                    conversationMessages.Add(msg);
                }
            }

            // Tạo payload với system instruction và conversation
            var contents = new List<object>();
            
            // Thêm system instruction nếu có
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                contents.Add(new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = systemPrompt }
                    }
                });
                
                // Thêm response từ model để acknowledge system instruction
                contents.Add(new
                {
                    role = "model",
                    parts = new[]
                    {
                        new { text = "I understand. I will help you recommend volunteer activities based on the provided list and user questions." }
                    }
                });
            }
            
            // Thêm conversation history
            foreach (var msg in conversationMessages)
            {
                contents.Add(new
                {
                    role = msg.Role == "user" ? "user" : "model",
                    parts = new[]
                    {
                        new { text = msg.Content }
                    }
                });
            }

            var payload = new
            {
                contents = contents.ToArray()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Debug: Log the payload
            Console.WriteLine($"Gemini Payload: {json}");

            var response = await _httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {jsonResponse}");

            using var doc = JsonDocument.Parse(jsonResponse);
            var reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return reply ?? "[Empty reply]";
        }
    }
}
