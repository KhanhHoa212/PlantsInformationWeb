using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Pages.Admin;

namespace PlantsInformationWeb.Pages
{
    public class Plants : PageModel
    {
        private readonly PlantService _plantService;

        public List<PlantSummaryDto> PlantsInfor { get; set; }


        public Plants(PlantService plantService)
        {
            _plantService = plantService;
        }

        public async Task OnGetAsync()
        {
            PlantsInfor = await _plantService.GetPlantsAsync();
        }

    }
}