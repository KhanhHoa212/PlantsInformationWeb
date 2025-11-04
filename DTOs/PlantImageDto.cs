using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantsInformationWeb.DTOs
{
    public class PlantImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}