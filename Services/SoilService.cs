using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Services
{
    public class SoilService
    {
        private readonly ISoilRepository _soilRepository;
        private readonly IGenericRepository<Plant> _plantRepo;
        private readonly IGenericRepository<Soiltype> _soilRepo;
        private readonly IMapper _mapper;

        public SoilService(IMapper mapper, ISoilRepository soilRepository, IGenericRepository<Plant> plantRepo, IGenericRepository<Soiltype> genericRepository)
        {
            _soilRepository = soilRepository;
            _plantRepo = plantRepo;
            _soilRepo = genericRepository;
            _mapper = mapper;
        }

        public async Task<List<Soiltype>> GetSoiltypesAsync()
        {
            return await _soilRepository.GetSoiltypesAsync();
        }

        public async Task<bool> AddSoilAsync(SoilViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SoilName) || string.IsNullOrWhiteSpace(model.Description) || string.IsNullOrWhiteSpace(model.FertilityLevel))
                return false;

            var soil = new Soiltype
            {
                SoilName = model.SoilName,
                PhRange = model.PhRange,
                FertilityLevel = model.FertilityLevel,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            await _soilRepo.AddAsync(soil);
            return true;
        }

        public async Task<bool> DeleteSoilAsync(int id)
        {
            var soil = await _soilRepo.GetByIdAsync(id);
            if (soil == null)
            {
                return false;
            }

            await _soilRepo.DeleteAsync(id);
            return true;
        }

        public async Task<List<string>> GetLinkedPlantBySoil(int soilId)
        {
            var plants = await _plantRepo.GetAllAsync();

            var linkedPlants = plants
                .Where(p => p.Soils.Any(s => s.SoilId == soilId))
                .Select(p => p.PlantName)
                .ToList();

            return linkedPlants;
        }

        public async Task<bool> UpdateSoilAsync(SoilViewModel model)
        {
            var soil = await _soilRepo.GetByIdAsync(model.SoilId);
            if (soil == null)
            {
                return false;
            }

            soil.SoilName = model.SoilName;
            soil.PhRange = model.PhRange;
            soil.FertilityLevel = model.FertilityLevel;
            soil.Description = model.Description;
            soil.UpdatedAt = DateTime.Now;

            await _soilRepo.UpdateAsync(soil);
            return true;
        }

        public async Task<List<Soiltype>> GetAllAsync()
        {
            var soiltypes = await _soilRepo.GetAllAsync();
            return soiltypes.ToList();
        }

        public async Task<int> GetTotalSoilsCountAsync()
        {
            return await _soilRepo.GetCountAsync();
        }

        public IQueryable<SoilTypeDto> GetSoilQuery(string search)
        {
            var query = _soilRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.ToLower();
                query = query.Where(s => s.SoilName.ToLower().Contains(keyword));
            }

            return query.ProjectTo<SoilTypeDto>(_mapper.ConfigurationProvider).OrderBy(s => s.SoilId);
        }
    }
}