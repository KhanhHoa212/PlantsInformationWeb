using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;

namespace PlantsInformationWeb.ViewModels
{
    public class AddPlantViewModel
    {
        public AddPlantRequestDto? PlantInfo { get; set; }
        public List<CategoryDto>? Categories { get; set; }
        public List<ClimateDto>? Climates { get; set; }
        public List<SoilTypeDto>? SoilTypes { get; set; }
        public List<RegionDto>? Regions { get; set; }
        public List<DiseaseDto>? Diseases { get; set; }
    }
}