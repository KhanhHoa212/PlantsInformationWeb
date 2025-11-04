using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class SoilTypeDto
    {
        public int SoilId { get; set; }

        public string SoilName { get; set; } = null!;

        public string? PhRange { get; set; }

        public string? FertilityLevel { get; set; }

        public string? Description { get; set; }
    }
}