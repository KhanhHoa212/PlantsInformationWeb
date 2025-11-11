using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class ChatMessageRequestDto
    {
        public int SessionId { get; set; }
        public string UserInput { get; set; }
    }
}