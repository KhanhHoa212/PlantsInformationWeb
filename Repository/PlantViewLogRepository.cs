using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class PlantViewLogRepository : IPlantViewLogRepository
    {
        private readonly PlantsInformationContext _context;

        public PlantViewLogRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<List<PlantViewLogDto>> GetTopViewedPlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5)
        {
            // Bắt đầu với IQueryable
            var query = _context.Plantviewlogs.AsQueryable();

            // Áp dụng điều kiện
            if (startDate.HasValue)
                query = query.Where(p => p.ViewedAt.HasValue && p.ViewedAt.Value.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(p => p.ViewedAt.HasValue && p.ViewedAt.Value.Date <= endDate.Value.Date);

            // Include ở cuối
            query = query.Include(v => v.Plant);

            var result = await query
                .GroupBy(v => new
                {
                    v.PlantId,
                    v.Plant.PlantName,
                    v.Plant.ScientificName,
                    CategoryName = v.Plant.Category != null ? v.Plant.Category.CategoryName : ""
                })
                .Select(g => new PlantViewLogDto
                {
                    PlantId = g.Key.PlantId ?? 0,
                    PlantName = g.Key.PlantName,
                    ScientificName = g.Key.ScientificName,
                    CategoryName = g.Key.CategoryName,
                    ViewCount = g.Count()
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(top)
                .ToListAsync();

            return result;
        }
    }
}