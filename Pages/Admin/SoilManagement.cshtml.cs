using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "admin")]
    public class SoilManagement : PageModel
    {
        private readonly ILogger<SoilManagement> _logger;
        private readonly SoilService _soilService;
        public PaginatedList<SoilTypeDto> Soiltypes { get; set; }
        [BindProperty]
        public SoilViewModel NewSoil { get; set; } = new();
        [BindProperty]
        public SoilViewModel EditSoil { get; set; } = new();

        public SoilManagement(ILogger<SoilManagement> logger, SoilService soilService)
        {
            _logger = logger;
            _soilService = soilService;
        }

        public async Task OnGetAsync(string search, int? pageIndex)
        {
            int pageSize = 10;
            var query = _soilService.GetSoilQuery(search);
            Soiltypes = await PaginatedList<SoilTypeDto>.CreateAsync(query, pageIndex ?? 1, pageSize);

        }

        public async Task<IActionResult> OnPostAddSoilAsync()
        {
            ModelState.Clear();
            TryValidateModel(NewSoil, nameof(NewSoil));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var result = await _soilService.AddSoilAsync(NewSoil);
            if (!result)
            {
                TempData["NotificationMessage"] = "Failed to add soil type.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            TempData["NotificationMessage"] = "Soil added successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteSoilAsync(int id)
        {
            var result = await _soilService.DeleteSoilAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Soil type not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Soid deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditSoilAsync()
        {
            ModelState.Clear();
            TryValidateModel(EditSoil, nameof(EditSoil));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Invalid input.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            var result = await _soilService.UpdateSoilAsync(EditSoil);
            if (!result)
            {
                TempData["NotificationMessage"] = "Soil not found or update failed.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Soil updated successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();

        }
    }
}