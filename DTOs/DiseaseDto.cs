using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class DiseaseDto
    {
        public int DiseaseId { get; set; }

        public string DiseaseName { get; set; } = null!;

        public string Symptoms { get; set; } = null!;

        public string Solution { get; set; } = null!;
    }
}