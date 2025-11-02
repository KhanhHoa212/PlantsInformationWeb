using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.DTOs;

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
        Plants = await _plantService.GetSixPlantsAsync();
    }
}
