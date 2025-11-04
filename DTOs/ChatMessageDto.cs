using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class ChatMessageDto
    {
        public int MessageId { get; set; }
        public int? SessionId { get; set; }
        public int? UserId { get; set; }
        public string? SenderType { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentAt { get; set; }
    }
}