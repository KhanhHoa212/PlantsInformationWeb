
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Repository
{
    public interface IPlantRepository
    {
        Task<int> GetCountAllPLantsAsync();

        Task<PaginatedList<PlantSummaryDto>> GetPlantSummariesAsync(int pageIndex, int pageSize);

        Task<Plant?> GetPlantByIdWithDetailsAsync(int id);

        Task<List<PlantSummaryDto>> GetAnySixPlantsAsync();
    }
}