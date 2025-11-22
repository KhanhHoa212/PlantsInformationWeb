using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Services;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    [IgnoreAntiforgeryToken]
    public class Dashboard : PageModel
    {
        private readonly ILogger<Dashboard> _logger;
        private readonly UserService _userService;
        private readonly PlantService _plantService;
        private readonly CategoryService _categoryService;
        private readonly RegionService _regionService;
        private readonly SoilService _soilService;
        private readonly DiseaseService _diseaseService;
        private readonly ClimateService _climateService;
        private readonly UnrecognizedPlantService _unrecognized;


        public int TotalUser { get; set; }
        public int TotalPlant { get; set; }
        public int TotalCategory { get; set; }
        public int TotalRegion { get; set; }
        public int TotalSoilType { get; set; }
        public int TotalDisease { get; set; }
        public int[] PlantAdditionsByMonth { get; set; }
        public PaginatedList<UnrecognizedplantDto> unPlant { get; set; }

        public Dashboard(ILogger<Dashboard> logger, UnrecognizedPlantService unrecognizedPlantService, ClimateService climateService, UserService userService, PlantService plantService, CategoryService categoryService, RegionService regionService, DiseaseService diseaseService, SoilService soilService)
        {
            _logger = logger;
            _userService = userService;
            _plantService = plantService;
            _categoryService = categoryService;
            _regionService = regionService;
            _soilService = soilService;
            _diseaseService = diseaseService;
            _climateService = climateService;
            _unrecognized = unrecognizedPlantService;
        }

        public async Task OnGetAsync(int? pageIndex)
        {
            int pageSize = 7;

            TotalUser = await _userService.GetTotalUsersCountAsync();
            TotalPlant = await _plantService.GetTotalPlantsAsync();
            TotalCategory = await _categoryService.GetTotalPlantsAsync();
            TotalSoilType = await _soilService.GetTotalSoilsCountAsync();
            TotalDisease = await _diseaseService.GetTotalDiseaseCountAsync();
            TotalRegion = await _regionService.GetTotalRegionCountAsync();

            unPlant = await _plantService.GetUnrecognizePlants(pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnGetPlantAdditionChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không truyền ngày, lấy cả năm hiện tại
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }

            var result = await _plantService.GetPlantAdditionByMonthAsync(startDate.Value, endDate.Value);

            return new JsonResult(new { labels = result.Labels, data = result.Data });
        }


        public async Task<IActionResult> OnGetUnrecognizedPlantsTableAsync(int pageIndex = 1)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            int pageSize = 7;
            var unPlant = await _plantService.GetUnrecognizePlants(pageIndex, pageSize);
            return Partial("_UnrecognizedPlantsTable", unPlant);
        }

        public async Task<PartialViewResult> OnPostDeleteUnrecognizedPlantAsync(int id, int? pageIndex)
        {
            var result = await _unrecognized.DeleteUnrecognizedPlantAsync(id);
            int pageSize = 7;
            var query = _unrecognized.GetUnrecognizedPlantQuery();
            var pagedList = await PaginatedList<UnrecognizedplantDto>.CreateAsync(query, pageIndex ?? 1, pageSize);

            int newPageIndex = pageIndex ?? 1;
            if (pagedList.Count == 0 && newPageIndex > 1)
            {
                newPageIndex--;
                pagedList = await PaginatedList<UnrecognizedplantDto>.CreateAsync(query, newPageIndex, pageSize);
            }

            return Partial("_UnrecognizedPlantsTable", pagedList);
        }

        public async Task<IActionResult> OnGetUserAdditionChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không truyền ngày, lấy cả năm hiện tại
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }

            var result = await _userService.GetUserAdditionByMonthAsync(startDate.Value, endDate.Value);

            return new JsonResult(new { labels = result.Labels, data = result.Data });
        }

        public async Task<JsonResult> OnGetCategoryPlantChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var data = await _categoryService.GetCategoriesWithPlantCountAsync(startDate, endDate);
            var labels = data.Select(x => x.CategoryName).ToList();
            var value = data.Select(x => x.PlantCount).ToList();
            return new JsonResult(new { labels, data = value });
        }

        public async Task<JsonResult> OnGetClimateChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không truyền ngày, lấy cả năm hiện tại
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }

            var result = await _climateService.GetClimatesWithPlantCountAsync(startDate.Value, endDate.Value);

            return new JsonResult(new { labels = result.Labels, data = result.Data });
        }

        public async Task<JsonResult> OnGetRegionChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }

            // Truyền filter xuống service
            var result = await _regionService.GetRegionListsAsync(startDate.Value, endDate.Value);
            return new JsonResult(new { labels = result.Labels, data = result.Data });
        }


    }
}