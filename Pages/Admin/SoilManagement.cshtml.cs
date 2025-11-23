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

        public async Task<IActionResult> OnPostAddSoilAsync(int? pageIndex)
        {
            ModelState.Clear();
            TryValidateModel(NewSoil, nameof(NewSoil));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Please fill in all required fields.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var checkedName = await _soilService.GetAllAsync();
            if(checkedName.Any(s => s.SoilName.Equals(NewSoil.SoilName, StringComparison.OrdinalIgnoreCase)))
            {
                
                TempData["NotificationMessage"] = "ERROR! The name of Soil Type is already existed.";
                TempData["NotificationType"] = "error";
                return RedirectToPage( new { pageIndex = pageIndex ?? 1 });
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
            return RedirectToPage(new { pageIndex = pageIndex ?? 1 });
        }

        public async Task<IActionResult> OnGetCheckLinkedPlants(int id)
        {
            var linkedPlants = await _soilService.GetLinkedPlantBySoil(id);

            return new JsonResult(new
            {
                hasLinkedPlants = linkedPlants.Count() > 0,
                plants = linkedPlants.ToList()
            });
        }

        public async Task<IActionResult> OnPostDeleteSoilAsync(int id, int? pageIndex)
        {
            var linkedPlants = await _soilService.GetLinkedPlantBySoil(id);
            if (linkedPlants.Count > 0)
            {
                TempData["NotificationMessage"] = $"Không thể xóa vì còn {linkedPlants.Count()} cây trồng liên kết.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var result = await _soilService.DeleteSoilAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Soil not found or could not be deleted.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            // Sau khi xóa, kiểm tra còn mấy dòng trên trang hiện tại
            int pageSize = 10;
            var query = _soilService.GetSoilQuery(Request.Query["search"]);
            var pagedList = await PaginatedList<SoilTypeDto>.CreateAsync(query, pageIndex ?? 1, pageSize);

            int newPageIndex = pageIndex ?? 1;
            if (pagedList.Count == 0 && newPageIndex > 1)
            {
                newPageIndex--;
            }

            TempData["NotificationMessage"] = "Soil deleted successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage(new { pageIndex = newPageIndex, search = Request.Query["search"].ToString() });
        }

        public async Task<IActionResult> OnPostEditSoilAsync(int? pageIndex)
        {
            ModelState.Clear();
            TryValidateModel(EditSoil, nameof(EditSoil));

            if (!ModelState.IsValid)
            {
                TempData["NotificationMessage"] = "Invalid input.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            var result = await _soilService.UpdateSoilAsync(EditSoil);
            if (!result)
            {
                TempData["NotificationMessage"] = "Soil not found or update failed.";
                TempData["NotificationType"] = "error";
                return Page();
            }

            TempData["NotificationMessage"] = "Soil updated successfully.";
            TempData["NotificationType"] = "success";
            return RedirectToPage(new { pageIndex = pageIndex ?? 1 });

        }
    }
}