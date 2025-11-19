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
    public class DiseaseService
    {
        private readonly IDiseaseRepository _diseaseRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Disease> _diseaseRepo;
        private readonly IGenericRepository<Plant> _plantRepo;


        public DiseaseService(IMapper mapper, IDiseaseRepository diseaseRepository, IGenericRepository<Plant> plantRepo, IGenericRepository<Disease> genericRepository)
        {
            _diseaseRepository = diseaseRepository;
            _diseaseRepo = genericRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _plantRepo = plantRepo;

        }

        internal async Task<List<Disease>> GetDiseasesAsync()
        {
            return (List<Disease>)await _diseaseRepo.GetAllAsync();
        }

        public async Task<bool> AddDiseaseAsync(DiseaseViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.DiseaseName) || string.IsNullOrWhiteSpace(model.Solution) || string.IsNullOrWhiteSpace(model.Symptoms))
                return false;

            var disease = new Disease
            {
                DiseaseName = model.DiseaseName,
                Solution = model.Solution,
                Symptoms = model.Symptoms,
                CreatedAt = DateTime.Now
            };

            await _diseaseRepo.AddAsync(disease);
            return true;
        }

        public async Task<bool> DeleteDiseaseAsync(int id)
        {
            var disease = await _diseaseRepo.GetByIdAsync(id);
            if (disease == null) return false;

            await _diseaseRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateDiseaseAsync(DiseaseViewModel model)
        {
            var disease = await _diseaseRepo.GetByIdAsync(model.DiseaseId);
            if (disease == null) return false;

            disease.DiseaseName = model.DiseaseName;
            disease.Solution = model.Solution;
            disease.Symptoms = model.Symptoms;
            disease.UpdatedAt = DateTime.Now;

            await _diseaseRepo.UpdateAsync(disease);
            return true;
        }

        public async Task<List<Disease>> GetAllAsync()
        {
            var diseases = await _diseaseRepo.GetAllAsync();
            return diseases.ToList();
        }

        public IQueryable<DiseaseDto> GetDiseaseQuery(string search)
        {
            var query = _diseaseRepo.GetQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.ToLower();
                query = query.Where(d => d.DiseaseName.ToLower().Contains(keyword));
            }
            return query.ProjectTo<DiseaseDto>(_mapper.ConfigurationProvider).OrderBy(d => d.DiseaseId);
        }
    
        public async Task<int> GetTotalDiseaseCountAsync()
        {
            return await _diseaseRepo.GetCountAsync();
        }

    }
}