using System.Text;
using EcoGreen.Helpers;
using EcoGreen.Service.Chat;
using Application.Entities.Base.AIChat;
using EcoGreen.Helpers;
using EcoGreen.Service.Chat;
using Application.Entities.Base.AIChat;
using Application.Entities.DTOs;
using Application.Interface.IServices;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoiceChatBotController : ControllerBase
    {
        private readonly string _geminiApiKey = "AIzaSyCQUYAzyAJ-oWyEkeYLA3gRCcwGOqtwcmM";
        private readonly AIChatService _aiChatService;
        private readonly ICompanyFormService _companyFormService;

        private static readonly ChatModel _chatModel = new ChatModel
        {
            Model = "gemini-2.0-flash",
            Messages = new List<Message>()
        };


        public VoiceChatBotController(ICompanyFormService companyFormService)
        {
            _companyFormService = companyFormService;
            _aiChatService = new AIChatService(_geminiApiKey);
            
            // Khởi tạo system prompt một lần duy nhất
            // _ = InitializeSystemPromptAsync();
        }

        [HttpPost("voice-chat")]
        public async Task<IActionResult> VoiceChat(IFormFile audioFile)
        //public async Task<IActionResult> VoiceChat([FromForm]string transcript)

        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("No audio file uploaded.");

            // 1. Lưu file tạm vào memory
            using var memoryStream = new MemoryStream();
            await audioFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // 2. Speech-to-Text
            var speechClient = SpeechClient.Create();
            var response = speechClient.Recognize(new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = "en-US"
            }, RecognitionAudio.FromBytes(memoryStream.ToArray()));

            var transcript = response.Results.FirstOrDefault()?.Alternatives.FirstOrDefault()?.Transcript;
            if (string.IsNullOrEmpty(transcript))
                return BadRequest("Could not recognize speech.");

            //3.Tạo prompt chuyên biệt và gửi đến Gemini
            string reply;

            try
            {
                // Kiểm tra xem đây có phải là tin nhắn đầu tiên không
                var isFirstMessage = _chatModel.Messages.Count == 0; // Chỉ có system prompt
                
                if (isFirstMessage)
                {
                    // Nếu là tin nhắn đầu tiên, cập nhật system prompt với context của user
                    await UpdateSystemPromptWithUserContext(transcript);
                }
                
                // Chỉ thêm tin nhắn người dùng vào lịch sử chat
                reply = await _aiChatService.ChatAsync(_chatModel, transcript);

                //return Ok(new {
                //    Response = reply,
                //    Conversation = _chatModel.Messages,
                //    Transcript = transcript,
                //    IsFirstMessage = isFirstMessage
                //});
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gemini Error: {ex.Message}");
            }

            // 4. Text-to-Speech (chia nhỏ nếu cần)
            var ttsClient = TextToSpeechClient.Create();
            var voice = new VoiceSelectionParams { LanguageCode = "en-US", SsmlGender = SsmlVoiceGender.Female };
            var config = new AudioConfig { AudioEncoding = AudioEncoding.Mp3 };

            var outputStream = new MemoryStream();
            foreach (var chunk in SplitTextByBytes(reply, 4900)) // an toàn < 5000 bytes
            {
                var ttsResponse = ttsClient.SynthesizeSpeech(new SynthesizeSpeechRequest
                {
                    Input = new SynthesisInput { Text = chunk },
                    Voice = voice,
                    AudioConfig = config
                });

                ttsResponse.AudioContent.WriteTo(outputStream);
            }

            outputStream.Position = 0;
            return File(outputStream.ToArray(), "audio/mpeg", "reply.mp3");
        }

        // ✅ Cập nhật system prompt với context của user và phân loại theo địa phương
        private async Task UpdateSystemPromptWithUserContext(string userTranscript)
        {
            try
            {
                // Lấy danh sách hoạt động
                var activityResponse = await _companyFormService.GetAllActivityFormsForAIVoice();
                var listOfActivityDto = new List<ActivityDTO>();
                
                if (activityResponse.isSuccess && activityResponse.Result != null)
                {
                    listOfActivityDto = (List<ActivityDTO>)activityResponse.Result;
                }

                // Tạo system prompt với context của user
                var enhancedSystemPrompt = $"""
You are a smart assistant that recommends volunteer activities. Be SHORT and DIRECT in responses (under 100 words).

User's request: "{userTranscript}"

Here is the current list of available activities:
{JsonConvert.SerializeObject(listOfActivityDto, Formatting.Indented)}

Rules:
1. Recommend only from the list above — do NOT make up activities.
2. If the user does NOT mention a location, politely ask: "Which city or province are you interested in volunteering?"
3. If the user does NOT mention a time or date, ask: "When would you like to volunteer? (e.g., this weekend, next month)"
4. If no exact match is found, suggest 1–2 nearby activities in neighboring locations (based on locations in the list).
5. If multiple matches, show up to 3 best ones.
6. Use this reply format ONLY:

"Recommended activities:

[Title]
- Description: [short]
- Location: [location]
- Date: [date]"

If nothing matches and no nearby options exist:
"No matching activities found. Try asking about different locations or interests."
""";


                // Cập nhật system prompt
                if (_chatModel.Messages.Count > 0 && _chatModel.Messages[0].Role == "system")
                {
                    _chatModel.Messages[0].Content = enhancedSystemPrompt;
                }
                else
                {
                    _chatModel.Messages.Insert(0, new Message 
                    { 
                        Role = "system", 
                        Content = enhancedSystemPrompt 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating system prompt: {ex.Message}");
            }
        }

        // ✅ Hàm chia văn bản theo byte UTF-8 an toàn
        private IEnumerable<string> SplitTextByBytes(string text, int maxBytes)
        {
            var encoding = Encoding.UTF8;
            var current = new StringBuilder();
            int currentBytes = 0;

            foreach (char c in text)
            {
                var charBytes = encoding.GetByteCount(new[] { c });
                if (currentBytes + charBytes > maxBytes)
                {
                    yield return current.ToString();
                    current.Clear();
                    currentBytes = 0;
                }

                current.Append(c);
                currentBytes += charBytes;
            }

            if (current.Length > 0)
                yield return current.ToString();
        }
    }
}
