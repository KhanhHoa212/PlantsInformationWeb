using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;

namespace PlantsInformationWeb.Services
{
    public class UnrecognizedPlantService
    {
        private readonly IGenericRepository<Unrecognizedplant> _unrecognized;
        private readonly IMapper _mapper;

        public UnrecognizedPlantService(IMapper mapper, IGenericRepository<Unrecognizedplant> unrecognizedPlant)
        {
            _mapper = mapper;
            _unrecognized = unrecognizedPlant;
        }

        public async Task<bool> DeleteUnrecognizedPlantAsync(int id)
        {
            var climate = await _unrecognized.GetByIdAsync(id);
            if (climate == null)
            {
                return false;
            }
            await _unrecognized.DeleteAsync(id);
            return true;
        }

        public IQueryable<UnrecognizedplantDto> GetUnrecognizedPlantQuery()
        {
            var query = _unrecognized.GetQueryable();
            
            return query.ProjectTo<UnrecognizedplantDto>(_mapper.ConfigurationProvider).OrderBy(d => d.UnrecognizedId);
        }
    }
}