using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.ViewModels;

public class PlantService
{
    private readonly IPlantRepository _plantRepository;

    private readonly IGenericRepository<Plant> _plantRepo;
    private readonly IGenericRepository<Disease> _diseaseRepo;
    private readonly IGenericRepository<Soiltype> _soilRepo;
    private readonly IGenericRepository<Region> _regionRepo;
    private readonly IMapper _mapper;

    private readonly ILogger<PlantService> _logger;

    public PlantService(ILogger<PlantService> logger, IGenericRepository<Region> regionRepo, IGenericRepository<Soiltype> soilRepo, IPlantRepository plantRepository, IGenericRepository<Plant> plantRepo, IMapper mapper, IGenericRepository<Disease> diseaseRepo)
    {
        _plantRepository = plantRepository;
        _plantRepo = plantRepo;
        _mapper = mapper;
        _diseaseRepo = diseaseRepo;
        _soilRepo = soilRepo;
        _regionRepo = regionRepo;
        _logger = logger;
    }

    public async Task<int> GetTotalPlantsAsync()
    {
        return await _plantRepository.GetCountAllPLantsAsync();
    }

    public async Task<List<Plant>> GetAllAsync()
    {
        var regions = await _plantRepo.GetAllAsync();
        return regions.ToList();
    }

    public async Task AddAsync(Plant plant)
    {
        await _plantRepo.AddAsync(plant);
    }

    public async Task AddPlantAsync(AddPlantRequestDto addPlantRequestDto)
    {
        var plant = _mapper.Map<Plant>(addPlantRequestDto);

        plant.CreatedAt = DateTime.Now;
        plant.Status = "Active";

        // Gán các liên kết nếu có
        if (addPlantRequestDto.DiseaseIds != null && addPlantRequestDto.DiseaseIds.Count > 0)
        {
            plant.Diseases = await _diseaseRepo.GetAllByIdAsync(addPlantRequestDto.DiseaseIds);
        }

        if (addPlantRequestDto.RegionIds != null && addPlantRequestDto.RegionIds.Count > 0)
        {
            plant.Regions = await _regionRepo.GetAllByIdAsync(addPlantRequestDto.RegionIds);
        }

        if (addPlantRequestDto.SoilTypeIds != null && addPlantRequestDto.SoilTypeIds.Count > 0)
        {
            plant.Soils = await _soilRepo.GetAllByIdAsync(addPlantRequestDto.SoilTypeIds);

        }
        if (addPlantRequestDto.ImageUrls != null && addPlantRequestDto.ImageUrls.Count > 0)
        {
            plant.Plantimages = addPlantRequestDto.ImageUrls
                .Select(url => new Plantimage
                {
                    ImageUrl = url,
                    IsPrimary = false
                }).ToList();
        }


        await _plantRepo.AddAsync(plant);
    }

    public async Task<bool> DeletePLantAsync(int id)
    {
        var plant = await _plantRepo.GetByIdAsync(id);
        if (plant == null) return false;

        await _plantRepo.DeleteAsync(id);
        return true;
    }

    public async Task<bool> EditPlantAsync(EditPlantRequestDto editPlantRequestDto)
    {

        var plant = await _plantRepository.GetPlantByIdWithDetailsAsync(editPlantRequestDto.PlantId);
        if (plant == null)
        {
            _logger.LogWarning($"Không tìm thấy cây với PlantId = {editPlantRequestDto.PlantId}");
            return false;
        }

        _mapper.Map(editPlantRequestDto, plant);

        // Update Diseases
        if (editPlantRequestDto.DiseaseIds != null)
        {
            var diseases = await _diseaseRepo.GetAllByIdAsync(editPlantRequestDto.DiseaseIds);
            plant.Diseases.Clear();
            foreach (var d in diseases)
                plant.Diseases.Add(d);
        }
        else
        {
            plant.Diseases.Clear();
        }

        // Update Regions
        if (editPlantRequestDto.RegionIds != null)
        {
            var regions = await _regionRepo.GetAllByIdAsync(editPlantRequestDto.RegionIds);
            plant.Regions.Clear();
            foreach (var r in regions)
                plant.Regions.Add(r);
        }
        else
        {
            plant.Regions.Clear();
        }

        // Update Soils
        if (editPlantRequestDto.SoilTypeIds != null)
        {
            var soils = await _soilRepo.GetAllByIdAsync(editPlantRequestDto.SoilTypeIds);
            plant.Soils.Clear();
            foreach (var s in soils)
                plant.Soils.Add(s);
        }
        else
        {
            plant.Soils.Clear();
        }
        await _plantRepo.UpdateAsync(plant);
        return true;
    }

    public IQueryable<PlantSummaryDto> GetPlantQuery(string search, int? categoryId)
    {
        var query = _plantRepo.GetQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.ToLower();
            query = query.Where(d => d.PlantName.ToLower().Contains(keyword));
        }
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(d => d.CategoryId == categoryId.Value);
        }
        return query.ProjectTo<PlantSummaryDto>(_mapper.ConfigurationProvider).OrderBy(d => d.PlantId);
    }

    public async Task<List<PlantSummaryDto>> GetSixPlantsAsync()
    {
        return await _plantRepository.GetAnySixPlantsAsync();
    }

    public async Task<PaginatedList<PlantSummaryDto>> GetPagedPlantsAsync(int pageIndex, int pageSize)
    {
        return await _plantRepository.GetPlantSummariesAsync(pageIndex, pageSize);
    }

}