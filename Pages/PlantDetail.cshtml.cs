using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Services;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Pages
{
    [Authorize(Roles = "admin,user")]
    [IgnoreAntiforgeryToken]
    public class PlantDetail : PageModel
    {
        private readonly ILogger<PlantDetail> _logger;
        private readonly PlantService _plantService;
        private readonly FavoritePlantService _favoriteService;
        private readonly PlantCommentService _plantComment;
        private readonly AIService _aiService;
        public bool IsFavorited { get; set; }
        public int? UserId { get; set; }

        public PlantViewModel Plant { get; set; }

        public PlantDetail(ILogger<PlantDetail> logger, AIService aiService, PlantCommentService plantCommentService, PlantService plantService, FavoritePlantService favoriteService)
        {
            _logger = logger;
            _plantService = plantService;
            _favoriteService = favoriteService;
            _plantComment = plantCommentService;
            _aiService = aiService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Plant = await _plantService.GetPlantById(id);
            if (Plant == null) return NotFound();

            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirst("user_id");
                if (claim != null && int.TryParse(claim.Value, out int userid))
                {
                    userId = userid;
                }
            }
            UserId = userId;

            await _plantService.IncreasePlantViewAsync(id, userId);

            if (userId.HasValue)
            {
                var favoriteDto = new FavoritePlantDto { UserId = userId.Value, PlantId = id };
                IsFavorited = await _favoriteService.IsFavoritedAsync(favoriteDto);
            }
            else
            {
                IsFavorited = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddFavoriteAsync(int plantId)
        {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new { success = false, message = "Not logged in" });

            var claim = User.FindFirst("user_id");
            if (claim == null || !int.TryParse(claim.Value, out int userId))
                return new JsonResult(new { success = false, message = "Invalid user" });

            var result = await _favoriteService.AddFavoritePlantAsync(new FavoritePlantDto { UserId = userId, PlantId = plantId });
            return new JsonResult(new { success = result });
        }

        public async Task<IActionResult> OnPostRemoveFavoriteAsync(int plantId)
        {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new { success = false, message = "Not logged in" });

            var claim = User.FindFirst("user_id");
            if (claim == null || !int.TryParse(claim.Value, out int userId))
                return new JsonResult(new { success = false, message = "Invalid user" });

            var result = await _favoriteService.DeleteFavoriteAsync(new FavoritePlantDto { UserId = userId, PlantId = plantId });
            return new JsonResult(new { success = result });
        }

        public async Task<IActionResult> OnGetCommentsAsync(int id)
        {
            var comments = await _plantComment.GetCommentsAsync(id);
            return new JsonResult(comments);
        }

        public async Task<IActionResult> OnPostAddCommentAsync([FromBody] PlantCommentDto plantComment)
        {
            var claim = User.FindFirst("user_id");
            if (claim == null || !int.TryParse(claim.Value, out int userId))
                return new JsonResult(new { success = false, message = "Invalid user" });
            plantComment.UserId = userId;

            var (isSafe, warning) = await _aiService.ModerateCommentAsync(plantComment.CommentText);
            if (!isSafe)
            {
                return new JsonResult(new { success = false, message = "Bình luận bị từ chối: " + warning });
            }

            var comment = await _plantComment.AddCommentAsync(plantComment);
            return new JsonResult(new { success = true, comment = comment });
        }
    }
}