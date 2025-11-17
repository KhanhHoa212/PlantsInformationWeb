using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Repository
{
    public class FavoritePlantRepository : IFavoriteRepository
    {
        private readonly PlantsInformationContext _context;
        private readonly IMapper _mapper;

        public FavoritePlantRepository(PlantsInformationContext plantsInformationContext, IMapper mapper)
        {
            _context = plantsInformationContext;
            _mapper = mapper;
        }
        public Task<bool> AddFavoriteAsync(int userId, int plantId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsFavoritedAsync(int userId, int plantId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFavoriteAsync(int userId, int plantId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FavoritePlantDto>> GetTopFavoritePlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5)
        {
            var query = _context.Favoriteplants.AsQueryable();
            if (startDate.HasValue)
                query = query.Where(p => p.FavoritedAt.HasValue && p.FavoritedAt.Value.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(p => p.FavoritedAt.HasValue && p.FavoritedAt.Value.Date <= endDate.Value.Date);
            query = query.Include(v => v.Plant);

            var result = await query
                .GroupBy(v => new
                {
                    v.PlantId,
                    v.Plant.PlantName,
                    v.Plant.ScientificName,
                    CategoryName = v.Plant.Category != null ? v.Plant.Category.CategoryName : ""
                })
                .Select(g => new FavoritePlantDto
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

        public async Task<PaginatedList<PlantSummaryDto>> GetFavoritePlantsAsync(int pageIndex, int pageSize, int userId)
        {
            var query = _context.Favoriteplants
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.FavoritedAt)
                .Select(f => f.Plant);

            var dtoQuery = _mapper.ProjectTo<PlantSummaryDto>(query);

            return await PaginatedList<PlantSummaryDto>.CreateAsync(dtoQuery.AsNoTracking(), pageIndex, pageSize);
        }
    }
}