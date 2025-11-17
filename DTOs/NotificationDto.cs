using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int PlantId { get; set; }

        public int? UserId { get; set; }

        public int? CommentId { get; set; }

        public string Message { get; set; } = null!;

        public bool? IsRead { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}