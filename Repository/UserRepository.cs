using System;
using System.Threading.Tasks;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using BCrypt.Net;

namespace PlantsInformationWeb.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly PlantsInformationContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(PlantsInformationContext context, ILogger<UserRepository> logger)
    : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Lấy toàn bộ thông tin users
        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        // Đếm số lượng người dùng
        public async Task<int> GetCountAllUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        // Truy vấn người dùng theo username và email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        // Lưu mã OTP vào bảng PasswordResetRequests
        public async Task SaveOtpAsync(int userId, string otp)
        {
            var request = new Passwordresetrequest
            {
                UserId = userId,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

            _context.Passwordresetrequests.Add(request);
            await _context.SaveChangesAsync();
        }

        //  Xác thực OTP
        public async Task<bool> VerifyOtpAsync(int userId, string otp)
        {
            var request = await _context.Passwordresetrequests
                .Where(r => r.UserId == userId && r.OtpCode == otp && r.IsUsed == false && r.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(r => r.ExpiresAt)
                .FirstOrDefaultAsync();

            return request != null;
        }

        //  Đánh dấu OTP đã dùng
        public async Task InvalidateOtpAsync(int userId)
        {
            var requests = await _context.Passwordresetrequests
                .Where(r => r.UserId == userId && r.IsUsed == false)
                .ToListAsync();

            foreach (var req in requests)
            {
                req.IsUsed = true;
            }

            await _context.SaveChangesAsync();
        }

        // Cập nhật mật khẩu mới
        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Không tìm thấy người dùng.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
            user.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();
        }

        // Kiểm tra mật khẩu hiện tại khi đổi mật khẩu
        public async Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
        }

        // Thêm user mới
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

    }
}
