using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Services;

namespace PlantsInformationWeb.Pages;

public class Index : PageModel
{
    private readonly ILogger<Index> _logger;
    private readonly PlantService _plantService;

    public List<PlantSummaryDto> Plants { get; set; }

    public Index(ILogger<Index> logger, PlantService plantService)
    {
        _logger = logger;
        _plantService = plantService;
    }

    public async Task OnGetAsync()
    {
        Plants = await _plantService.GetHotPlantsAsync(8);

        var userId = User.FindFirst("user_id")?.Value;
        Console.WriteLine("UserId in claims: " + userId);

    }
}