using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Globalization;
using System.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using PlantsInformationWeb.Hubs;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<User> _userRepo;
    private readonly ILogger<UserService> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IGenericRepository<User> userRepo, IHubContext<NotificationHub> hubContext)
    {
        _userRepository = userRepository;
        _userRepo = userRepo;
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await _userRepository.GetCountAllUsersAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepo.GetByIdAsync(userId);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<bool> AddUserAsync(UserViewModel model)
    {
        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return false;
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            Role = model.Role,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow.ToLocalTime(),
            Isactive = true
        };
        await _userRepository.AddUserAsync(user);
        return true;
    }

    public async Task<bool> ToggleUserActiveStatusAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return false;

        user.Isactive = !(user.Isactive ?? false);
        await _userRepo.UpdateAsync(user);

        if (user.Isactive == false)
        {
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("AccountLocked", "Your account has been locked by admin");
        }

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


    public async Task<(string[] Labels, int[] Data)> GetUserAdditionByMonthAsync(DateTime startDate, DateTime endDate)
    {
        return await _userRepository.GetUserCreateByMonthAsync(startDate, endDate);
    }

    public async Task<byte[]> ExportUserRegistrationsPdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
    {
        var users = await _userRepository.GetUserRegistrationsByMonthAsync(startDate, endDate);

        byte[] chartImgBytes = null;
        if (!string.IsNullOrWhiteSpace(chartImageBase64))
        {
            var base64 = chartImageBase64.Substring(chartImageBase64.IndexOf(',') + 1);
            chartImgBytes = Convert.FromBase64String(base64);
        }

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text("List login account")
                    .FontSize(20).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    if (chartImgBytes != null)
                    {
                        col.Item().Image(chartImgBytes).FitWidth();
                        col.Item().PaddingBottom(10);
                    }

                    col.Item().Table(table =>
                    {

                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                        });


                        table.Header(header =>
                        {
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("ID");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("UsserName");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Email");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Role");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Status");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Registed at");
                        });

                        foreach (var user in users)
                        {
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(user.UserId.ToString());
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(user.Username ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(user.Email ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(user.Role ?? "");
                            var statusText = user.Isactive == true ? "Active" : "Inactive";
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(statusText);
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(
                                user.CreatedAt.HasValue ? user.CreatedAt.Value.ToString("dd/MM/yyyy") : ""
                            );
                        }
                    });

                    col.Item().Element(e => e.PaddingTop(8)).Text($"Sum of account: {users.Count}").FontSize(14).Bold().AlignRight();
                });

                page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            });
        });

        return doc.GeneratePdf();
    }
}
