using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class RegionRepository : IRegionRepository
    {
        private readonly PlantsInformationContext _context;

        public RegionRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAllRegionsAsync()
        {
            return await _context.Regions.CountAsync();
        }

        public async Task<(string[] Labels, int[] Data)> GetRegionListsAsync(DateTime startDate, DateTime endDate)
        {
            // Lấy các plant trong khoảng ngày
            var plantsInRange = await _context.Plants
                .Where(p => p.CreatedAt.HasValue
                    && p.CreatedAt.Value.Date >= startDate.Date
                    && p.CreatedAt.Value.Date <= endDate.Date)
                .Include(p => p.Regions)
                .ToListAsync();

            // Đếm số cây theo từng Region
            var regionCounts = plantsInRange
                .SelectMany(p => p.Regions)
                .GroupBy(r => r.RegionId)
                .Select(g => new
                {
                    RegionId = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Lấy tất cả các Region
            var regions = await _context.Regions.ToListAsync();

            // Chuẩn bị labels (tên vùng) và data (số lượng cây theo vùng)
            var labels = regions.Select(r => r.RegionName).ToArray();
            var counts = regions.Select(r =>
            {
                var found = regionCounts.FirstOrDefault(x => x.RegionId == r.RegionId);
                return found != null ? found.Count : 0;
            }).ToArray();

            return (labels, counts);
        }

        public async Task<List<RegionDto>> GetPlantDistributionByRegionAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Regions
                .Include(r => r.Plants)
                .AsQueryable();

            // Nếu muốn lọc cây theo ngày tạo
            if (startDate.HasValue || endDate.HasValue)
            {
                query = query.Select(r => new Region
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName,
                    CreatedAt = r.CreatedAt,
                    Plants = r.Plants
                        .Where(p => (!startDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value >= startDate.Value))
                                 && (!endDate.HasValue || (p.CreatedAt.HasValue && p.CreatedAt.Value <= endDate.Value)))
                        .ToList()
                });
            }

            var result = await query
                .Select(r => new RegionDto
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName,
                    PlantCount = r.Plants.Count,
                    CreatedAt = r.CreatedAt
                })
                .OrderBy(r => r.RegionName)
                .ToListAsync();

            return result;
        }

        public async Task<RegionDto> GetTopRegionAsync()
        {
            var result = await _context.Regions.Select(r => new RegionDto
            {
                RegionName = r.RegionName,
                PlantCount = r.Plants.Count()
            }).OrderByDescending(r => r.PlantCount).FirstOrDefaultAsync();

            return result;
        }

    }
}