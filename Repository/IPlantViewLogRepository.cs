using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;

namespace PlantsInformationWeb.Repository
{
    public interface IPlantViewLogRepository
    {
        Task<List<PlantViewLogDto>> GetTopViewedPlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5);
    }
}