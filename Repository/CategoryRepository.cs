
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

        public async Task<List<CategoryWithCountDto>> GetCategoriesWithPlantCountAsync(DateTime? startDate, DateTime? endDate)
        {
            // Lấy tất cả plants theo điều kiện ngày
            var plantQuery = _context.Plants.AsQueryable();

            if (startDate.HasValue)
                plantQuery = plantQuery.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                plantQuery = plantQuery.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Date <= endDate.Value.Date);

            // Nhóm theo Category
            var categoryCounts = await plantQuery
                .Where(p => p.CategoryId != null)
                .GroupBy(p => p.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    PlantCount = g.Count()
                })
                .ToListAsync();

            // Lấy toàn bộ category, sau đó join với categoryCounts trên bộ nhớ
            var categories = await _context.Plantcategories.ToListAsync();

            var result = (from c in categories
                          join cc in categoryCounts on c.CategoryId equals cc.CategoryId into ccGroup
                          from cc in ccGroup.DefaultIfEmpty()
                          select new CategoryWithCountDto
                          {
                              CategoryId = c.CategoryId,
                              CategoryName = c.CategoryName,
                              Description = c.Description,
                              PlantCount = cc != null ? cc.PlantCount : 0
                          }).ToList();

            return result;
        }

        public async Task<CategoryDto> GetTop1CategoryAsync()
        {
            var topCategory = await _context.Plantcategories
                  .Select(c => new CategoryDto
                  {
                      CategoryId = c.CategoryId,
                      CategoryName = c.CategoryName,
                      PlantCount = c.Plants.Count
                  })
                    .OrderByDescending(c => c.PlantCount)
                    .FirstOrDefaultAsync();

            return topCategory;
        }

        public async Task<List<CategoryDto>> GetPlantDistributionByCategoryAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Plants
                .Where(p => p.CategoryId != null
                    && (!startDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value >= startDate.Value))
                    && (!endDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value <= endDate.Value)));

            var result = await query
                .GroupBy(p => new { p.CategoryId, p.Category.CategoryName })
                .Select(g => new CategoryDto
                {
                    CategoryId = g.Key.CategoryId ?? 0,
                    CategoryName = g.Key.CategoryName,
                    PlantCount = g.Count()
                })
                .OrderBy(x => x.CategoryName)
                .ToListAsync();

            return result;
        }

    }
}