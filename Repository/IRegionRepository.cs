using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;

namespace PlantsInformationWeb.Repository
{
    public interface IRegionRepository
    {
        Task<(string[] Labels, int[] Data)> GetRegionListsAsync(DateTime startDate, DateTime endDate);
        Task<List<RegionDto>> GetPlantDistributionByRegionAsync(DateTime? startDate, DateTime? endDate);

        Task<RegionDto> GetTopRegionAsync();

        Task<int> GetCountAllRegionsAsync();

    }
}