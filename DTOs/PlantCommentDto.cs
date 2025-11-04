using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.DTOs
{
    public class PlantCommentDto
    {
        public int CommentId { get; set; }

        public int PlantId { get; set; }

        public int? UserId { get; set; }

        public int? ParentCommentId { get; set; }

        public string CommentText { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; }
        public string? UserName { get; set; }
        public List<PlantCommentDto> Replies { get; set; } = new List<PlantCommentDto>();
    }
}