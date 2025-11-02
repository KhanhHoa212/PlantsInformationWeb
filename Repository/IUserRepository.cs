using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface IUserRepository
    {
        // Lấy toàn bộ thông tin users
        Task<List<User>> GetUsersAsync();

        // Đếm ng dùng
        Task<int> GetCountAllUsersAsync();

        // Kiểm tra thông tin người dùng khi quên mật khẩu
        Task<User?> GetByEmailAsync(string email);

        // Lưu mã OTP 
        Task SaveOtpAsync(int userId, string otp);

        //Xác thực  OTP
        Task<bool> VerifyOtpAsync(int userId, string otp);

        // Cập nhật mật khẩu mới
        // Task UpdatePasswordAsync(int userId, string newPassword);

        //  Đánh dấu OTP đã dùng hoặc xoá
        Task InvalidateOtpAsync(int userId);

        // Kiểm tra pass cũ khi đổi pass sau khi đăng nhập
        Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword);
        
        // Thêm user mới
        Task AddUserAsync(User user);
    }
}
    