
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PlantsInformationContext _context;


        public CategoryRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAllCategoryAsync()
        {
            return await _context.Plantcategories.CountAsync();
        }

        public async Task<List<CategoryWithCountDto>> GetCategoriesWithPlantCountAsync()
        {
            var result = await _context.Plantcategories
                .Select(c => new CategoryWithCountDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    PlantCount = c.Plants.Count // EF sẽ đếm số lượng cây liên kết
                })
                .ToListAsync();

            return result;
        }

      
    }
}