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
    [IgnoreAntiforgeryToken]
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
            var query = _userService.GetUserQuery(search, role, status);
            Users = await PaginatedList<User>.CreateAsync(query, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid infor."
                });
            }

            var success = await _userService.AddUserAsync(NewUser);
            if (!success)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Email is already existed."
                });
            }

            return new JsonResult(new
            {
                success = true,
                message = "Add user successful."
            });
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var currentUserIdStr = User.FindFirst("user_id")?.Value;
            int currentUserId = 0;
            if (!string.IsNullOrEmpty(currentUserIdStr))
                int.TryParse(currentUserIdStr, out currentUserId);

            if (id == currentUserId)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "You cannot lock your own account!"
                });
            }
            var result = await _userService.ToggleUserActiveStatusAsync(id);
            if (!result)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            var user = await _userService.GetUsersAsync();
            var updatedUser = user.FirstOrDefault(x => x.UserId == id);

            return new JsonResult(new
            {
                success = true,
                message = "Account status has been updated.",
                newStatus = updatedUser?.Isactive
            });

        }
    }
}