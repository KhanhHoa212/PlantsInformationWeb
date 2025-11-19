using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;

namespace PlantsInformationWeb.ViewModels
{
    public class PlantViewModel
    {
        public int PlantId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PlantName { get; set; }
        public string? ScientificName { get; set; }
        public string? CategoryName { get; set; }
        public ClimateDto Climate { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        //public List<int> RegionIds { get; set; } = new();
        public string? GrowthCycle { get; set; }
        public int CategoryId { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; }

        public List<DiseaseDto> Disease { get; set; } = new();
        public List<SoilTypeDto> SoilType { get; set; } = new();
        public List<UsageDto> Usages { get; set; } = new();
        public List<RegionDto> Regions { get; set; } = new();
        public List<PlantImageDto> PlantImages { get; set; } = new();

    }
}