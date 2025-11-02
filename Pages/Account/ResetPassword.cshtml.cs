using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.Repository;

namespace PlantsInformationWeb.Pages.Account;

public class ResetPasswordModel : PageModel
{
    [BindProperty]
    public string? NewPassword { get; set; }

    [BindProperty]
    public string? ConfirmPassword { get; set; }

    private readonly IUserRepository _userRepo;

    public ResetPasswordModel(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (NewPassword != ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");
            return Page();
        }

        if (!TempData.ContainsKey("UserId"))
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng.");
            return Page();
        }

        var userId = Convert.ToInt32(TempData["UserId"]);
        await _userRepo.UpdatePasswordAsync(userId, NewPassword!);

        TempData["SuccessMessage"] = "Mật khẩu đã được cập nhật.";
        return RedirectToPage("/Account/Login");
    }
}
