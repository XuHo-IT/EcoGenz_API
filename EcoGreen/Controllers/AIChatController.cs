﻿using Application.Entities.Base;
using Application.Request.Chat;
using Application.Response.Chat;
using EcoGreen.Service;
using EcoGreen.Service.Chat;
using Microsoft.AspNetCore.Mvc;

namespace EcoGreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : ControllerBase
    {
        private readonly AIChatService _chatService;
        private readonly VisionService _visionService;

        public AIChatController(AIChatService chatService, VisionService visionService)
        {
            _chatService = chatService;
            _visionService = visionService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required.");

            var messages = new List<Message>
    {
        new Message
        {
            Role = "system",
            Content = @"You are EcoGenz, a chill and super helpful AI who’s all about helping people take action for the environment — like organizing clean-ups, planting trees, recycling right, or spreading awareness.  
You talk like a friendly teenager: casual, energetic, and relatable. Give real-life examples, step-by-step tips, and ideas that are easy to follow. Keep things short, clear, and fun — use emojis, slang (when it fits), and make it feel like a convo with a cool eco buddy.  
Always hype people up to take action and ask more questions. You’re here to make saving the planet easy and awesome. 🌍💚"
        }
    };
            if (request.PreviousMessages != null)
            {
                messages.AddRange(request.PreviousMessages.Where(m =>
                    m.Role != null && m.Role.ToLower() != "system" &&
                    !string.IsNullOrWhiteSpace(m.Content)));
            }

            var model = new ChatModel
            {
                Model = "tinyllama:latest",
                Messages = messages
            };

            string response = await _chatService.ChatAsync(model, request.Message);

            return Ok(new ChatResponse
            {
                Response = response,
                Conversation = model.Messages
            });
        }

        [HttpPost("recycle-image")]
        public async Task<IActionResult> RecycleImage(IFormFile? image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Image is required.");

            string base64Image;
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            string detectedItem = await _visionService.DetectObjectFromImage(base64Image);

            if (string.IsNullOrEmpty(detectedItem))
                return BadRequest("Could not detect item in image.");

            var messages = new List<Message>
    {
        new Message
        {
            Role = "system",
            Content = @"You are EcoGenz, a helpful recycling assistant. Give short and clear tips on how to recycle items properly. Respond casually like a friendly teenager."
        },
        new Message
        {
            Role = "user",
            Content = $"How do I recycle a {detectedItem}?"
        }
    };

            var model = new ChatModel
            {
                Model = "tinyllama:latest",
                Messages = messages
            };

            string tip = await _chatService.ChatAsync(model, messages.Last().Content);

            return Ok(new
            {
                DetectedItem = detectedItem,
                RecycleTip = tip
            });
        }

    }
}