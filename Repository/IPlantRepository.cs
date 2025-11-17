
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Repository
{
    public interface IPlantRepository
    {
        Task<int> GetCountAllPLantsAsync();

        Task<PaginatedList<PlantSummaryDto>> GetPlantSummariesAsync(int pageIndex, int pageSize);
        Task<PaginatedList<UnrecognizedplantDto>> GetUnrecognizedplantAsync(int pageIndex, int pageSize);

        Task<Plant?> GetPlantByIdWithDetailsAsync(int id);

        Task<(string[] Labels, int[] Data)> GetPlantAddByMonthAsync(DateTime startDate, DateTime endDate);
        Task<List<PlantSummaryDto>> GetPlantAdditionByMonthAsync(DateTime? startDate, DateTime? endDate);
        Task<int> LogSearchPlantAsync(int? userId, string keyword, object filterObj);
        Task LogSearchPlantResultsAsync(int searchId, List<int> plantIds);

        Task<List<int>> GetHotPlantIdsAsync(int topN);
        Task<List<PlantSummaryDto>> GetPlantSummariesByIdsAsync(List<int> plantIds);

        Task RemoveAllImagesOfPlantAsync(int plantId);
        Task<List<Plant>> GetAllPlantsWithDetailsAsync();
        
    }
}