using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.ViewModels
{
    public class ClimateViewModel
    {
        public int ClimateId { get; set; }

        [Required]
        public string ClimateName { get; set; } = string.Empty;

        [Required]
        public string TemperatureRange { get; set; } = string.Empty;

        [Required]
        public string RainfallRange { get; set; } = string.Empty;

        [Required]
        public string HumidityRange { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}