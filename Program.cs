using PlantsInformationWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
// using PlantsInformationWeb.Pages.Account;
using PlantsInformationWeb.Repository;
// using PlantsInformationWeb.Services;
using QuestPDF.Infrastructure;
using PlantsInformationWeb.Hubs;
using Microsoft.AspNetCore.SignalR;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.Middlewares;
using PlantsInformationWeb.Pages.Account;
using PlantsInformationWeb.Services;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.
builder.Services.AddRazorPages()
    .AddSessionStateTempDataProvider()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    }); builder.Services.AddSession();


// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<PlantsInformationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<ISoilRepository, SoilRepository>();
builder.Services.AddScoped<IClimateRepository, ClimateRepository>();
builder.Services.AddScoped<IDiseaseRepository, DiseaseRepository>();
builder.Services.AddScoped<IPlantViewLogRepository, PlantViewLogRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoritePlantRepository>();
builder.Services.AddScoped<IPlantCommentRepository, PlantCommentRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<PlantViewLogRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UnrecognizedPlantService>();
builder.Services.AddScoped<PlantViewLogService>();
builder.Services.AddScoped<PlantService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<RegionService>();
builder.Services.AddScoped<SoilService>();
builder.Services.AddScoped<ClimateService>();
builder.Services.AddScoped<DiseaseService>();
builder.Services.AddScoped<FavoritePlantService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<PlantCommentService>();
// builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddControllers();


builder.Services.AddSignalR();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapHub<NotificationHub>("/notificationHub");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<AccountLockMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
