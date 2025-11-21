using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Pages.Admin;
using PlantsInformationWeb.Services;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Pages
{
    public class Plants : PageModel
    {
        private readonly PlantService _plantService;
        private readonly CategoryService _categoryService;
        private readonly ClimateService _climateService;
        private readonly SoilService _soilService;
        private readonly FavoritePlantService _favoriteService;
        private readonly ChatService _chatService;

        private readonly IMapper _mapper;

        public PaginatedList<PlantSummaryDto> PlantsInfor { get; set; }

        public List<CategoryViewModel> Categories { get; set; }
        public List<ClimateViewModel> Climates { get; set; }
        public List<SoilViewModel> Soils { get; set; }

        [BindProperty]
        public string UserInput { get; set; }

        public int CurrentSessionId { get; set; }
        public List<ChatMessageDto> ChatHistory { get; set; }
        public int UserId { get; set; }


        public Plants(FavoritePlantService favoritePlantService, ChatService chatService, PlantService plantService, CategoryService categoryService, IMapper mapper, ClimateService climateService, SoilService soilService)
        {
            _plantService = plantService;
            _categoryService = categoryService;
            _mapper = mapper;
            _climateService = climateService;
            _soilService = soilService;
            _favoriteService = favoritePlantService;
            _chatService = chatService;
        }

        public async Task OnGetAsync(
            string keyword,
            int pageIndex = 1,
            int pageSize = 20,
            List<int>? categoryIds = null,
            List<int>? climateIds = null,
            List<int>? soilIds = null,
            bool favorite = false)
        {
            Categories = _mapper.Map<List<CategoryViewModel>>(await _categoryService.GetAllAsync());
            Climates = _mapper.Map<List<ClimateViewModel>>(await _climateService.GetClimatesAsync());
            Soils = _mapper.Map<List<SoilViewModel>>(await _soilService.GetSoiltypesAsync());

            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirst("user_id");
                if (claim != null && int.TryParse(claim.Value, out int id))
                {
                    userId = id;
                }
            }

            if (favorite && userId.HasValue)
            {
                PlantsInfor = await _favoriteService.GetFavoritePlantsPagedAsync(userId.Value, pageIndex, pageSize);

            }
            else
            {
                PlantsInfor = await _plantService.SearchPlantsPagedAsync(keyword, pageIndex, pageSize, categoryIds, climateIds, soilIds);

                var plantIds = PlantsInfor.Select(p => p.PlantId).ToList();
                await _plantService.LogPlantSearchAsync(userId, keyword, categoryIds, climateIds, soilIds, plantIds);
            }
        }
    }

}