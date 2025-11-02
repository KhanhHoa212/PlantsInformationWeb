using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class UserManagement : PageModel
    {
        private readonly UserService _userService;

        private readonly IMapper _mapper;
        private readonly ILogger<UserManagement> _logger;

        public PaginatedList<User> Users { get; set; }
        [BindProperty]
        public UserViewModel NewUser { get; set; } = new UserViewModel();

        public UserManagement(IMapper mapper, ILogger<UserManagement> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task OnGetAsync(string search, string role, string status, int? pageIndex)
        {
            int pageSize = 10;
            var query = _userService.GetUserQuery( search,  role,  status);
            Users = await PaginatedList<User>.CreateAsync(query, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["NotificationType"] = "error";
                TempData["NotificationMessage"] = "Invalid infor.";
                return Page();
            }

            var success = await _userService.AddUserAsync(NewUser);
            if (!success)
            {
                TempData["NotificationType"] = "error";
                TempData["NotificationMessage"] = "Email is already existed.";
                return Page();
            }

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Add user successful.";
            return Page();
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var result = await _userService.ToggleUserActiveStatusAsync(id);
            if (!result)
            {
                TempData["NotificationMessage"] = "Không tìm thấy người dùng.";
                TempData["NotificationType"] = "error";
                return RedirectToPage();
            }

            TempData["NotificationMessage"] = "Trạng thái tài khoản đã được cập nhật.";
            TempData["NotificationType"] = "success";
            return RedirectToPage();
        }

    }
}