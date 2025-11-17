using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface ICategoryRepository
    {
        Task<int> GetCountAllCategoryAsync();
        Task<List<CategoryWithCountDto>> GetCategoriesWithPlantCountAsync(DateTime? startDate, DateTime? endDate);
        Task<List<CategoryDto>> GetPlantDistributionByCategoryAsync(DateTime? startDate, DateTime? endDate);

        Task<CategoryDto> GetTop1CategoryAsync();
    }
}