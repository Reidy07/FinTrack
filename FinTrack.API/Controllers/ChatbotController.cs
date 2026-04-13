using FinTrack.Core.Constants;
using FinTrack.Core.DTOs.Chatbot;
using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IAIChatbotService _chatService;

        public ChatbotController(IAIChatbotService chatService)
        {
            _chatService = chatService;
        }
        [HttpPost("ask")]
        public async Task<ActionResult<ChatResponseDto>> Ask([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.UserMessage))
                return BadRequest(ErrorMessages.IncompleteData);

            var reply = await _chatService.AskFinancialAdvisorAsync(request.UserId, request.UserMessage);
            return Ok(new ChatResponseDto { Reply = reply });
        }
    }
}