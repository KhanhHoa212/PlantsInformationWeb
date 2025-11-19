using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Services;
using PlantsInformationWeb.Utils;
 
namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class PlantManagement : PageModel
    {
        private readonly ILogger<PlantManagement> _logger;
        private readonly IMapper _mapper;
 
        public List<Region> Regions { get; set; } = new();
        public List<Climate> Climates { get; set; } = new();
        public List<Disease> Diseases { get; set; } = new();
        public List<Soiltype> SoilTypes { get; set; } = new();
        public List<Plantcategory> Categories { get; set; } = new();
 
        //public List<PlantSummaryDto> PlasntSummaries { get; set; } = new();
        public PaginatedList<PlantSummaryDto> PlantSummaries { get; set; }
        public List<CategoryWithCountDto> CategoriesWithCount { get; set; } = new();
 
        public readonly RegionService _regionService;
        public readonly ClimateService _climateService;
        public readonly DiseaseService _diseaseService;
        public readonly SoilService _soilService;
        public readonly CategoryService _categoryService;
        public readonly PlantService _plantService;
 
        [BindProperty]
        public AddPlantRequestDto AddPlant { get; set; } = new();
        [BindProperty]
        public EditPlantRequestDto EditPlant { get; set; } = new();
        public bool ShowAddModal { get; set; } = false;
 
        public PlantManagement(ILogger<PlantManagement> logger, PlantService plantService, IMapper mapper, RegionService regionService, ClimateService climateService, DiseaseService diseaseService, SoilService soilService, CategoryService categoryService)
        {
            _logger = logger;
            _regionService = regionService;
            _categoryService = categoryService;
            _climateService = climateService;
            _diseaseService = diseaseService;
            _soilService = soilService;
            _mapper = mapper;
            _plantService = plantService;
        }
 
        public async Task OnGetAsync(string? search, int? pageIndex, int? categoryId, string? plantName, string? showAddModal)
        {
            Regions = await _regionService.GetAllAsync();
            Climates = await _climateService.GetAllAsync();
            Diseases = await _diseaseService.GetAllAsync();
            SoilTypes = await _soilService.GetAllAsync();
            Categories = await _categoryService.GetAllAsync();
 
            int pageSize = 10;
            ShowAddModal = !string.IsNullOrEmpty(showAddModal) && showAddModal.ToLower() == "true";
            if (!string.IsNullOrEmpty(plantName))
            {
                AddPlant.PlantName = plantName;
            }
 
            List<int>? categoryIds = null;
            if (categoryId.HasValue)
                categoryIds = new List<int> { categoryId.Value };
 
            var query = _plantService.GetPlantQuery(search, categoryIds, null, null)
                .ProjectTo<PlantSummaryDto>(_mapper.ConfigurationProvider);
 
            PlantSummaries = await PaginatedList<PlantSummaryDto>.CreateAsync(query, pageIndex ?? 1, pageSize);
        }
 
        public async Task<IActionResult> OnPostAddPLantAsync(int? pageIndex)
        {
            ModelState.Clear();
            TryValidateModel(AddPlant, nameof(AddPlant));
 
            if (AddPlant.ImageUrls != null && AddPlant.ImageUrls.Count == 1)
            {
                var raw = AddPlant.ImageUrls[0];
                AddPlant.ImageUrls = raw
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }
 
            if (!ModelState.IsValid || AddPlant == null)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }
 
            var checkedName = await _plantService.GetAllAsync();
            if (checkedName.Any(s => s.PlantName.Equals(AddPlant.PlantName, StringComparison.OrdinalIgnoreCase)))
            {
 
                TempData["NotificationMessage"] = "ERROR! The name of Plant is already existed.";
                TempData["NotificationType"] = "error";
                return RedirectToPage(new { pageIndex = pageIndex ?? 1 });
            }
 
            try
            {
                await _plantService.AddPlantAsync(AddPlant);
 
                TempData["NotificationMessage"] = "Plant added successfully.";
                TempData["NotificationType"] = "success";
                return RedirectToPage(new { pageIndex = pageIndex ?? 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plant.");
                TempData["NotificationMessage"] = "An error occurred while adding the plant.";
                TempData["NotificationType"] = "error";
                return Page();
            }
        }
        public async Task<IActionResult> OnPostDeletePlantAsync(int id, int? pageIndex)
        {
            var result = await _plantService.DeletePLantAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Plant not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return Page();
            }
 
            int pageSize = 10;
            var search = Request.Query["search"].ToString();
            var query = _plantService.GetPlantQuery(search, null, null, null);
            var pagedList = await PaginatedList<Plant>.CreateAsync(query, pageIndex ?? 1, pageSize);
 
            int newPageIndex = pageIndex ?? 1;
            // Nếu trang hiện tại không còn cây nào và không phải trang đầu, chuyển về trang trước
            if (pagedList.Count == 0 && newPageIndex > 1)
            {
                newPageIndex--;
            }
 
            TempData["NotificationMessage"] = "Plant deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage(new { pageIndex = newPageIndex, search = search });
        }
 
        public async Task<IActionResult> OnPostEditPlant(int? pageIndex)
        {
 
            ModelState.Clear();
            TryValidateModel(EditPlant, nameof(EditPlant));
 
            if (EditPlant.ImageUrls != null && EditPlant.ImageUrls.Count == 1)
            {
                var raw = EditPlant.ImageUrls[0];
                EditPlant.ImageUrls = raw
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }
 
            if (!ModelState.IsValid || EditPlant == null)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }
 
            try
            {
                await _plantService.EditPlantAsync(EditPlant);
 
                TempData["NotificationMessage"] = "Plant edited successfully.";
                TempData["NotificationType"] = "success";
                return RedirectToPage(new { pageIndex = pageIndex ?? 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plant.");
                TempData["NotificationMessage"] = "An error occurred while editing the plant.";
                TempData["NotificationType"] = "error";
                await OnGetAsync(null, null, null, null, null);
                return Page();
            }
        }
    }
 
}