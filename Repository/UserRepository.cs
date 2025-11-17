using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using BCrypt.Net;
using PlantsInformationWeb.DTOs;

namespace PlantsInformationWeb.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly PlantsInformationContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(PlantsInformationContext context, ILogger<UserRepository> logger) : base(context)
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

        public async Task<(string[] Labels, int[] Data)> GetUserCreateByMonthAsync(DateTime startDate, DateTime endDate)
        {
            // Lấy tất cả user có CreatedAt trong khoảng ngày
            var query = _context.Users
                .Where(u => u.CreatedAt.HasValue
                    && u.CreatedAt.Value.Date >= startDate.Date
                    && u.CreatedAt.Value.Date <= endDate.Date);

            // Group theo tháng/năm trong khoảng ngày
            var data = await query
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            // Chuẩn bị labels và data
            var labels = new List<string>();
            var counts = new List<int>();

            // Duyệt từ tháng bắt đầu đến tháng kết thúc
            DateTime iter = new DateTime(startDate.Year, startDate.Month, 1);
            DateTime endIter = new DateTime(endDate.Year, endDate.Month, 1);

            while (iter <= endIter)
            {
                labels.Add($"{iter:MMM yyyy}");
                var found = data.FirstOrDefault(x => x.Year == iter.Year && x.Month == iter.Month);
                counts.Add(found != null ? found.Count : 0);
                iter = iter.AddMonths(1);
            }

            return (labels.ToArray(), counts.ToArray());
        }

        public async Task<List<UserDto>> GetUserRegistrationsByMonthAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Users.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value <= endDate.Value);

            return await query.OrderBy(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    Isactive = u.Isactive
                })
                .ToListAsync();
        }
    }
}
