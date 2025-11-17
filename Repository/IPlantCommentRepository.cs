using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;

namespace PlantsInformationWeb.Repository
{
    public interface IPlantCommentRepository
    {
        Task<List<PlantCommentDto>> GetCommentsByPlantId(int plantId);

        Task<Plantcomment> GetCommentWithUserByIdAsync(int commentId);

    }
}