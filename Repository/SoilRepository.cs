using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class SoilRepository : ISoilRepository
    {
        private readonly PlantsInformationContext _context;

        public SoilRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<List<Soiltype>> GetSoiltypesAsync()
        {
            return await _context.Soiltypes.ToListAsync();
        }
    }
}