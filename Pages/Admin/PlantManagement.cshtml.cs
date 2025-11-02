using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        //public List<PlantSummaryDto> PlantSummaries { get; set; } = new();
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

        public async Task OnGetAsync(string search, int? pageIndex, int? categoryId)
        {
            Regions = await _regionService.GetAllAsync();
            Climates = await _climateService.GetAllAsync();
            Diseases = await _diseaseService.GetAllAsync();
            SoilTypes = await _soilService.GetAllAsync();
            Categories = await _categoryService.GetAllAsync();
            //PlantSummaries = await _plantService.GetPlantSummariesAsync();
            CategoriesWithCount = await _categoryService.GetCategoriesWithPlantCountAsync();

            int pageSize = 10;
            var query = _plantService.GetPlantQuery(search, categoryId);
            PlantSummaries = await PaginatedList<PlantSummaryDto>.CreateAsync(query, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAddPLantAsync()
        {
            ModelState.Clear();
            TryValidateModel(AddPlant, nameof(AddPlant));

            if (!ModelState.IsValid || AddPlant == null)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            try
            {
                await _plantService.AddPlantAsync(AddPlant);

                TempData["NotificationMessage"] = "Plant added successfully.";
                TempData["NotificationType"] = "success";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plant.");
                TempData["NotificationMessage"] = "An error occurred while adding the plant.";
                TempData["NotificationType"] = "error";
                return Page();
            }
        }
        public async Task<IActionResult> OnPostDeletePLantAsync(int id)
        {
            var result = await _plantService.DeletePLantAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Plant not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Plant deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditPlant()
        {

            ModelState.Clear();
            TryValidateModel(EditPlant, nameof(EditPlant));

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
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plant.");
                TempData["NotificationMessage"] = "An error occurred while editing the plant.";
                TempData["NotificationType"] = "error";
                return Page();
            }
        }
    }

}