using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class EditPlantRequestDto
    {
        public int PlantId { get; set; }
        public string? PlantName { get; set; }
        public string? ScientificName { get; set; }
        public int CategoryId { get; set; }
        public int ClimateId { get; set; }
        public List<int>? SoilTypeIds { get; set; }
        public List<int>? RegionIds { get; set; }
        public List<int>? DiseaseIds { get; set; }
         public List<string>? ImageUrls { get; set; }
        public string? GrowthCycle { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; }
        //public DateTime? UpdatedAt { get; set; }
    }
}