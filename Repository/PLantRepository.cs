
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Pages;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Repository
{
    public class PlantRepository : IPlantRepository
    {
        private readonly PlantsInformationContext _context;


        public PlantRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAllPLantsAsync()
        {
            return await _context.Plants.CountAsync();
        }

        public async Task<PaginatedList<PlantSummaryDto>> GetPlantSummariesAsync(int pageIndex, int pageSize)
        {
            var query = _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .Include(p => p.Plantimages)
                .OrderBy(p => p.PlantId)
                .Select(p => new PlantSummaryDto
                {
                    PlantId = p.PlantId,
                    ThumbnailUrl = p.Plantimages.FirstOrDefault().ImageUrl,
                    PlantName = p.PlantName,
                    ScientificName = p.ScientificName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Không rõ",
                    ClimateName = p.Climate != null ? p.Climate.ClimateName : "Không rõ",
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    Description = p.Description
                });

            return await PaginatedList<PlantSummaryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<UnrecognizedplantDto>> GetUnrecognizedplantAsync(int pageIndex, int pageSize)
        {
            var query = _context.Unrecognizedplants
                .OrderByDescending(u => u.Createdat)
                .Select(u => new UnrecognizedplantDto
                {
                    UnrecognizedId = u.UnrecognizedId,
                    Plantname = u.Plantname,
                    Createdat = u.Createdat,
                    Usermessage = u.Usermessage

                });

            return await PaginatedList<UnrecognizedplantDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<Plant?> GetPlantByIdWithDetailsAsync(int id)
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .Include(p => p.Soils)
                .Include(p => p.Regions)
                .Include(p => p.Diseases)
                .Include(p => p.Plantimages)
                .Include(p => p.Usages)
                .FirstOrDefaultAsync(p => p.PlantId == id);
        }

        public async Task<(string[] Labels, int[] Data)> GetPlantAddByMonthAsync(DateTime startDate, DateTime endDate)
        {
            var query = _context.Plants
                .Where(p => p.CreatedAt.HasValue
                && p.CreatedAt.Value.Date >= startDate.Date
                && p.CreatedAt.Value.Date <= endDate.Date);

            var data = await query
                .GroupBy(p => new { p.CreatedAt.Value.Year, p.CreatedAt.Value.Month })
                .Select(f => new
                {
                    Year = f.Key.Year,
                    Month = f.Key.Month,
                    Count = f.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var labels = new List<string>();
            var counts = new List<int>();

            // Duyệt từ tháng bắt đầu đến tháng kết thúc
            DateTime iter = new DateTime(startDate.Year, startDate.Month, 1);
            DateTime endIter = new DateTime(endDate.Year, endDate.Month, 1);

            while (iter <= endIter)
            {
                labels.Add($"{iter:MMM yyyy}");
                var found = data.FirstOrDefault(x => x.Year == iter.Year && x.Month == iter.Month);
                counts.Add(found != null ? found.Count : 0);
                iter = iter.AddMonths(1);
            }

            return (labels.ToArray(), counts.ToArray());
        }

        public async Task<List<PlantSummaryDto>> GetPlantAdditionByMonthAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Plants.AsQueryable();
            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value <= endDate.Value);

            return await query.OrderBy(p => p.CreatedAt)
                .Select(p => new PlantSummaryDto
                {
                    PlantId = p.PlantId,
                    PlantName = p.PlantName,
                    ScientificName = p.ScientificName,
                    Origin = p.Origin,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    GrowthCycle = p.GrowthCycle,
                    CreatedAt = p.CreatedAt
                }).ToListAsync();
        }

        public async Task<int> LogSearchPlantAsync(int? userId, string keyword, object filterObj)
        {
            var log = new Plantsearchlog
            {
                UserId = userId,
                Keyword = keyword,
                SearchedAt = DateTime.Now,
                FilterJson = JsonSerializer.Serialize(filterObj)
            };
            _context.Plantsearchlogs.Add(log);
            await _context.SaveChangesAsync();
            return log.SearchId;
        }

        public async Task LogSearchPlantResultsAsync(int searchId, List<int> plantIds)
        {
            if (plantIds == null) plantIds = new List<int>();
            var resultLogs = plantIds.Select(pid => new Plantsearchresultlog
            {
                SearchId = searchId,
                PlantId = pid
            }).ToList();

            _context.Plantsearchresultlogs.AddRange(resultLogs);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetHotPlantIdsAsync(int topN)
        {
            return await _context.Plantviewlogs
                .GroupBy(l => l.PlantId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<List<PlantSummaryDto>> GetPlantSummariesByIdsAsync(List<int> plantIds)
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .Include(p => p.Plantimages)
                .Where(p => plantIds.Contains(p.PlantId))
                .Select(p => new PlantSummaryDto
                {
                    PlantId = p.PlantId,
                    ThumbnailUrl = p.Plantimages.FirstOrDefault().ImageUrl,
                    PlantName = p.PlantName,
                    ScientificName = p.ScientificName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Không rõ",
                    ClimateName = p.Climate != null ? p.Climate.ClimateName : "Không rõ",
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    Description = p.Description
                })
                .ToListAsync();
        }

        public async Task RemoveAllImagesOfPlantAsync(int plantId)
        {
            var images = _context.Plantimages.Where(x => x.PlantId == plantId);
            _context.Plantimages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Plant>> GetAllPlantsWithDetailsAsync()
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .Include(p => p.Regions)
                .Include(p => p.Soils)
                .Include(p => p.Diseases)
                .ToListAsync();
        }

    }
}