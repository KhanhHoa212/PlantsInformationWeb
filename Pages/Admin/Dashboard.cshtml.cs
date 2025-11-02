using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class Dashboard : PageModel
    {
        private readonly ILogger<Dashboard> _logger;
        private readonly UserService _userService;
        private readonly PlantService _plantService;
        private readonly CategoryService _categoryService;

        public int TotalUser { get; set; }
        public int TotalPlant { get; set; }
        public int TotalCategory { get; set; }

        public Dashboard(ILogger<Dashboard> logger, UserService userService, PlantService plantService, CategoryService categoryService)
        {
            _logger = logger;
            _userService = userService;
            _plantService = plantService;
            _categoryService = categoryService;
        }

        public async Task OnGetAsync()
        {
            TotalUser = await _userService.GetTotalUsersCountAsync();
            TotalPlant = await _plantService.GetTotalPlantsAsync();
            TotalCategory = await _categoryService.GetTotalPlantsAsync();
        }
    }
}