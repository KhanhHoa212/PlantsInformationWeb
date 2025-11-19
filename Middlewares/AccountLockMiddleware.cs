using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace PlantsInformationWeb.Middlewares
{
    public class AccountLockMiddleware
    {
        private readonly RequestDelegate _next;

        public AccountLockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            // Chỉ kiểm tra với request đã đăng nhập
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = context.User.FindFirst("user_id")?.Value;
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))

                {
                    // Lấy dữ liệu user từ DB (bạn có thể inject service qua DI)
                    var user = await userService.GetUserByIdAsync(userId);

                    if (user != null && user.Isactive == false)
                    {
                        await context.SignOutAsync();

                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}