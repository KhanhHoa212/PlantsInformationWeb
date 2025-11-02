
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
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

        // public async Task<List<PlantSummaryDto>> GetPlantSummariesAsync(int? count = null)
        // {
        //     var query = _context.Plants
        //         .Include(p => p.Category)
        //         .Include(p => p.Climate)
        //         .Include(p => p.Plantimages)
        //         .AsQueryable();

        //     if (count.HasValue && count.Value > 0)
        //     {
        //         query = query.Take(count.Value);
        //     }

        //     var result = await query
        //         .Select(p => new PlantSummaryDto
        //         {
        //             PlantId = p.PlantId,
        //             ThumbnailUrl = p.Plantimages.FirstOrDefault().ImageUrl,
        //             PlantName = p.PlantName,
        //             ScientificName = p.ScientificName,
        //             CategoryName = p.Category != null ? p.Category.CategoryName : "Không rõ",
        //             ClimateName = p.Climate != null ? p.Climate.ClimateName : "Không rõ",
        //             CreatedAt = p.CreatedAt,
        //             Status = p.Status,
        //             Description = p.Description
        //         })
        //         .ToListAsync();

        //     return result;
        // }

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

        public async Task<Plant?> GetPlantByIdWithDetailsAsync(int id)
        {
            return await _context.Plants
                .Include(p => p.Diseases)
                .Include(p => p.Regions)
                .Include(p => p.Soils)
                .Include(p => p.Plantimages)
                .FirstOrDefaultAsync(p => p.PlantId == id);
        }

        public async Task<List<PlantSummaryDto>> GetAnySixPlantsAsync()
        {
            var query = _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .Include(p => p.Plantimages)
                .OrderBy(p => p.PlantId)
                .Take(6)
                .Select(p => new PlantSummaryDto
                {
                    PlantId = p.PlantId,
                    ThumbnailUrl = p.Plantimages.FirstOrDefault().ImageUrl,
                    PlantName = p.PlantName,
                    ScientificName = p.ScientificName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Không rõ",
                    Description = p.Description
                });

            return await query.ToListAsync();
        }

    }
}