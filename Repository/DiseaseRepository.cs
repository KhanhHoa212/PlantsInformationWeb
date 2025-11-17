using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public class DiseaseRepository : IDiseaseRepository
    {
        private readonly PlantsInformationContext _context;

        public DiseaseRepository(PlantsInformationContext context)
        {
            _context = context;
        }

        
    }
}