using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class PlantViewLogDto
    {
        public int PlantId { get; set; }
        public string? PlantName { get; set; }
        public string? ScientificName { get; set; }
        public string? CategoryName { get; set; }
        public int ViewCount { get; set; }
    }
}