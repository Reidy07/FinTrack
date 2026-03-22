using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Core.DTOs.Chatbot
{
    public class ChatRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserMessage { get; set; } = string.Empty;
    }
}
