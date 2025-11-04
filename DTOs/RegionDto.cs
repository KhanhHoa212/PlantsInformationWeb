using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class RegionDto
    {
        public int RegionId { get; set; }

        public string RegionName { get; set; } = null!;

        public string? Description { get; set; }
        
        public int PlantCount { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
