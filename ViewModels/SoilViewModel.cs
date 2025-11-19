using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.ViewModels
{
    public class SoilViewModel
    {
        public int SoilId { get; set; }

        [Required(ErrorMessage = "Soil name is required.")]
        public string SoilName { get; set; } = string.Empty;

        [Required(ErrorMessage = "PhRange is required.")]
        public string PhRange { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fertility Level is required.")]
        public string FertilityLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;


    }
}