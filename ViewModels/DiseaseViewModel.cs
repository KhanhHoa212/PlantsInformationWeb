using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.ViewModels
{
    public class DiseaseViewModel
    {
        public int DiseaseId { get; set; }

        [Required]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        public string Symptoms { get; set; } = string.Empty;

        [Required]
        public string Solution { get; set; } = string.Empty;
    }
}