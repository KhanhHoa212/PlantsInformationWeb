using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface IClimateRepository
    {
        Task<List<Climate>> GetClimatetypesAsync();
        //Task<List<ClimateDto>> GetClimateToChartWithPlantCountAsync();
        Task<(string[] Labels, int[] Data)> GetClimateToChartWithPlantCountAsync(DateTime startDate, DateTime endDate);
        Task<List<ClimateDto>> GetPlantDistributionByClimateAsync(DateTime? startDate, DateTime? endDate);

    }
}