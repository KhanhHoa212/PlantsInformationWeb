using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.ViewModels
{
    public class UsageViewModel
    {
        public int UsageId { get; set; }

        public int? PlantId { get; set; }

        public string UsageType { get; set; } = null!;

        public string Details { get; set; } = null!;

        public virtual Plant? Plant { get; set; }
    }
}