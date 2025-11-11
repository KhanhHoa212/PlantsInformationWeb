using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class ChatSessionDto
    {
        public int SessionId { get; set; }

        public int? UserId { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

    }
}