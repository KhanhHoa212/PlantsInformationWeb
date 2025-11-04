using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.DTOs
{
    public class UsageDto
    {
        public int UsageId { get; set; }

        public int? PlantId { get; set; }

        public string UsageType { get; set; } = null!;

        public string Details { get; set; } = null!;
    }
}