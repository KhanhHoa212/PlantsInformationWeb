using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using PlantsInformationWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Services;

namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    [IgnoreAntiforgeryToken]
    public class ReportsSectionModel : PageModel
    {
        private readonly PlantsInformationContext _context;
        private readonly CategoryService _categoryService;
        private readonly RegionService _regionService;
        private readonly PlantService _plantService;
        private readonly UserService _userService;
        private readonly ClimateService _climateService;
        private readonly FavoritePlantService _favoriteService;
        private readonly PlantViewLogService _plantViewLogService;

        public CategoryDto TopCategory { get; set; }
        public double TopCategoryPercent { get; set; }
        public RegionDto TopRegion { get; set; }
        public int TotalPlant { get; set; }
        public int TotalUser { get; set; }
        public double TopRegionPercent { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ExportFormat { get; set; }

        public ReportsSectionModel(PlantsInformationContext context, FavoritePlantService favoritePlantService, PlantViewLogService plantViewLogService, ClimateService climateService, UserService userService, PlantService plantService, CategoryService categoryService, RegionService regionService)
        {
            _context = context;
            _categoryService = categoryService;
            _regionService = regionService;
            _plantService = plantService;
            _userService = userService;
            _climateService = climateService;
            _plantViewLogService = plantViewLogService;
            _favoriteService = favoritePlantService;
        }

        public async Task OnGetAsync()
        {
            var caregory = await _categoryService.GetTop1CategoryWithPercentAsync();
            TopCategory = caregory.TopCategory;
            TopCategoryPercent = caregory.percent;

            var region = await _regionService.GetTop1RegionWithPercentAsync();
            TopRegion = region.TopRegion;
            TopRegionPercent = region.percent;

            TotalPlant = await _plantService.GetTotalPlantsAsync();
            TotalUser = await _userService.GetTotalUsersCountAsync();
        }

        public async Task<IActionResult> OnGetPlantAdditionChartDataAsync(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }
            var result = await _plantService.GetPlantAdditionByMonthAsync(startDate.Value, endDate.Value);
            return new JsonResult(new { labels = result.Labels, data = result.Data });
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
            // Nếu không truyền ngày, lấy cả năm hiện tại
            if (!startDate.HasValue || !endDate.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }

            var data = await _categoryService.GetCategoriesWithPlantCountAsync(startDate.Value, endDate.Value);
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
            // Nếu không truyền ngày, lấy cả năm hiện tại
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
        public async Task<JsonResult> OnGetTopViewedPlantsAsync(DateTime? startDate, DateTime? endDate, int? top)
        {
            int topCount = top ?? 10;

            var result = await _plantViewLogService.GetTopViewedPlantsAsync(startDate, endDate, topCount);

            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetTopFavoritePlantsAsync(DateTime? startDate, DateTime? endDate, int? top)
        {
            int topCount = top ?? 10;

            var result = await _favoriteService.GetTopFavoritePlantsAsync(startDate, endDate, topCount);

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostExportUserRegistrationsAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _userService.ExportUserRegistrationsPdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"user_registrations_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantAdditionAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _plantService.ExportPlantAdditionPdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_addition_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantDistributionAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _regionService.ExportPlantDistributionPdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_distribution_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantDistributionByCategoryAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _categoryService.ExportPlantDistributionByCategoryPdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_distribution_category_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantDistributionByClimateAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _climateService.ExportPlantDistributionByClimatePdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_distribution_climate_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantViewAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _plantViewLogService.ExportPlantViewPdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_distribution_climate_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

        public async Task<IActionResult> OnPostExportPlantFavoriteAsync(DateTime? startDate, DateTime? endDate, string format, string chartImage)
        {
            if (format?.ToLower() == "pdf")
            {
                var fileBytes = await _favoriteService.ExportPlantFavoritePdfAsync(startDate, endDate, chartImage);
                return File(fileBytes, "application/pdf", $"plant_favorite_view_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            return BadRequest("Unsupported format");
        }

    }
}