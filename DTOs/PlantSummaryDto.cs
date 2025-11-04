using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class PlantSummaryDto
    {
        public int PlantId { get; set; }
        public string? ThumbnailUrl { get; set; }
       public List<string> ImageUrls { get; set; } = new();

        public string? PlantName { get; set; }
        public string? ScientificName { get; set; }
        public string? CategoryName { get; set; }
        public string? ClimateName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }

        public List<int> SoilTypeIds { get; set; } = new();
        public List<int> RegionIds { get; set; } = new();
        public List<int> DiseaseIds { get; set; } = new();
        public string? GrowthCycle { get; set; }
        public int CategoryId { get; set; }
        public int ClimateId { get; set; }
        
        public string? Origin { get; set; }
        public string? Description { get; set; }
        public int PlantCount { get; set; }

    }
}