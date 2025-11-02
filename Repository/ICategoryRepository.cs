using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface ICategoryRepository
    {
        Task<int> GetCountAllCategoryAsync();

        Task<List<CategoryWithCountDto>> GetCategoriesWithPlantCountAsync();
        // Task AddAsync(Plantcategory category);
    }
}