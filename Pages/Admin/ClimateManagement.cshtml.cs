using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Services;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Pages.Admin
{
    public class ClimateManagement : PageModel
    {
        private readonly ILogger<ClimateManagement> _logger;
        private readonly ClimateService _climateService;
        [BindProperty]
        public ClimateViewModel NewClimate { get; set; } = new();
        [BindProperty]
        public ClimateViewModel EditClimate { get; set; } = new();
        public PaginatedList<ClimateDto> Climates { get; set; }

        public ClimateManagement(ILogger<ClimateManagement> logger, ClimateService climateService)
        {
            _logger = logger;
            _climateService = climateService;
        }

        public async Task OnGetAsync(string search, int? pageIndex)
        {
            int pageSize = 10;
            var query = _climateService.GetClimateQuery(search);
            Climates = await PaginatedList<ClimateDto>.CreateAsync(query, pageIndex ?? 1, pageSize);
            //Climates = await _climateService.GetClimatesAsync();
        }

        public async Task<IActionResult> OnPostAddClimateAsync()
        {
            ModelState.Clear();
            TryValidateModel(NewClimate, nameof(NewClimate));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var result = await _climateService.AddClimateAsync(NewClimate);
            if (!result)
            {
                TempData["NotificationMessage"] = "Failed to add climate.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            TempData["NotificationMessage"] = "Climate added successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteClimateAsync(int id)
        {
            var result = await _climateService.DeleteClimateAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Climate not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Climate deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditClimateAsync()
        {
            ModelState.Clear();
            TryValidateModel(EditClimate, nameof(EditClimate));
            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Invalid input.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            var result = await _climateService.UpdateClimateAsync(EditClimate);
            if (!result)
            {
                TempData["NotificationMessage"] = "Climate not found or update failed.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Climate updated successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }
    }
}