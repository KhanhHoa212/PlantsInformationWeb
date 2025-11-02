using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Models;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<User> _userRepo;
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IGenericRepository<User> userRepo)
    {
        _userRepository = userRepository;
        _userRepo = userRepo;
        _logger = logger;
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await _userRepository.GetCountAllUsersAsync();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    // public async Task<bool> AddUserAsync(UserViewModel model)
    // {
    //     var existingUser = await _userRepository.GetByEmailAsync(model.Email);
    //     if (existingUser != null)
    //     {
    //         return false;
    //     }
    //     var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
    //     var user = new User
    //     {
    //         Username = model.Username,
    //         Email = model.Email,
    //         Role = model.Role,
    //         PasswordHash = hashedPassword,
    //         CreatedAt = DateTime.UtcNow.ToLocalTime(),
    //         Isactive = true
    //     };
    //     await _userRepository.AddUserAsync(user);
    //     return true;
    // }

    public async Task<bool> ToggleUserActiveStatusAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return false;

        user.Isactive = !(user.Isactive ?? false);
        await _userRepo.UpdateAsync(user);
        return true;
    }

    public IQueryable<User> GetUserQuery(string search, string role, string status)
    {
        var query = _userRepo.GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Username.ToLower().Contains(search.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status == "active")
                query = query.Where(u => u.Isactive == true);
            else if (status == "inactive")
                query = query.Where(u => u.Isactive == false);
        }

        return query.OrderBy(u => u.Username);
    }
}
