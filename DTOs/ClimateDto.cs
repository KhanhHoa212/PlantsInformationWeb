using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class ClimateDto
    {
        public int ClimateId { get; set; }
        public string ClimateName { get; set; } = null!;

        public string? TemperatureRange { get; set; }

        public string? RainfallRange { get; set; }

        public string? HumidityRange { get; set; }

        public string? Description { get; set; }

        public int PlantCount { get; set; }
    }
}