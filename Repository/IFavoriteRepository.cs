using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Utils;

namespace PlantsInformationWeb.Repository
{
    public interface IFavoriteRepository
    {
        Task<bool> IsFavoritedAsync(int userId, int plantId);
        Task<List<FavoritePlantDto>> GetTopFavoritePlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5);

        Task<PaginatedList<PlantSummaryDto>> GetFavoritePlantsAsync(int pageIndex, int pageSize, int userId);
    }
}