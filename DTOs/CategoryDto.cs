using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public int PlantCount { get; set; }
    }
}