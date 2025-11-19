using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.ViewModels
{
    public class RegionViewModel
    {
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Region name is required.")]
        public string RegionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
    }
}