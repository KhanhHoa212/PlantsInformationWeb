using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class CategoryWithCountDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PlantCount { get; set; }
    }
}