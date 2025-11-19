    using Microsoft.EntityFrameworkCore;
    using BCrypt.Net;
    using PlantsInformationWeb.Models;

    namespace PlantsInformationWeb.Pages.Account
    {
        public class AuthService
        {
            private readonly PlantsInformationContext _context;

            public AuthService(PlantsInformationContext context)
            {
                _context = context;
            }

            public async Task<User?> AuthenticateAsync(string email, string password)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null) return null;

                // So sánh mật khẩu hash
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return null;

                return user;
            }

            public async Task<User?> GetUserByEmailAsync(string email)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }

            public async Task<bool> RegisterUserAsync(string email, string username, string password)
            {
                // Mã hóa mật khẩu bằng BCrypt trong C#
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new User
                {
                    Email = email,
                    Username = username,
                    PasswordHash = hashedPassword,
                    Role = "user"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return true;
            }


        }
    }
