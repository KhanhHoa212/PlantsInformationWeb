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
    public class DiseasesManagement : PageModel
    {
        private readonly ILogger<DiseasesManagement> _logger;
        private readonly DiseaseService _diseaseService;
        public PaginatedList<DiseaseDto> Diseases { get; set; }

        [BindProperty]
        public DiseaseViewModel NewDisease { get; set; } = new();

        [BindProperty]
        public DiseaseViewModel EditDisease { get; set; } = new();

        public DiseasesManagement(ILogger<DiseasesManagement> logger, DiseaseService diseaseService)
        {
            _logger = logger;
            _diseaseService = diseaseService;
        }

        public async Task OnGetAsync(string search, int? pageIndex)
        {
            int pageSize = 10;
            var query = _diseaseService.GetDiseaseQuery(search);
            Diseases = await PaginatedList<DiseaseDto>.CreateAsync(query, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAddDiseaseAsync()
        {
            ModelState.Clear();
            TryValidateModel(NewDisease, nameof(NewDisease));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var result = await _diseaseService.AddDiseaseAsync(NewDisease);
            if (!result)
            {
                TempData["NotificationMessage"] = "Failed to add disease.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            TempData["NotificationMessage"] = "Disease added successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteDiseaseAsync(int id)
        {
            var result = await _diseaseService.DeleteDiseaseAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Disease not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Disease deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditDiseaseAsync()
        {
            ModelState.Clear();
            TryValidateModel(EditDisease, nameof(EditDisease));
            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Invalid input.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            var result = await _diseaseService.UpdateDiseaseAsync(EditDisease);
            if (!result)
            {
                TempData["NotificationMessage"] = "Disease not found or update failed.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Disease updated successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }
    }
}