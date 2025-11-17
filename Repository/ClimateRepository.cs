using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Repository
{
    public class ClimateRepository : IClimateRepository
    {
        private readonly PlantsInformationContext _context;

        public ClimateRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<List<Climate>> GetClimatetypesAsync()
        {
            return await _context.Climates.ToListAsync();
        }

        public async Task<(string[] Labels, int[] Data)> GetClimateToChartWithPlantCountAsync(DateTime startDate, DateTime endDate)
        {
            // Truy vấn bảng Plants, lọc theo khoảng ngày
            var plantQuery = _context.Plants
                .Where(p => p.CreatedAt.HasValue
                    && p.CreatedAt.Value.Date >= startDate.Date
                    && p.CreatedAt.Value.Date <= endDate.Date);

            // Group theo ClimateId và đếm số lượng cây
            var plantCounts = await plantQuery
                .Where(p => p.ClimateId != null)
                .GroupBy(p => p.ClimateId)
                .Select(g => new
                {
                    ClimateId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Lấy tất cả loại khí hậu
            var climates = await _context.Climates.ToListAsync();

            // Chuẩn bị labels (tên khí hậu) và data (số lượng cây trồng theo khí hậu)
            var labels = climates.Select(c => c.ClimateName).ToArray();
            var counts = climates.Select(c =>
            {
                var found = plantCounts.FirstOrDefault(x => x.ClimateId == c.ClimateId);
                return found != null ? found.Count : 0;
            }).ToArray();

            return (labels, counts);
        }

        public async Task<List<ClimateDto>> GetPlantDistributionByClimateAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Plants
                .Where(p => p.ClimateId != null
                    && (!startDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value >= startDate.Value))
                    && (!endDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value <= endDate.Value)));

            var result = await query
                .GroupBy(p => new { p.ClimateId, p.Climate.ClimateName })
                .Select(g => new ClimateDto
                {
                    ClimateId = g.Key.ClimateId ?? 0,
                    ClimateName = g.Key.ClimateName,
                    PlantCount = g.Count()
                })
                .OrderBy(x => x.ClimateName)
                .ToListAsync();

            return result;
        }

    }
}