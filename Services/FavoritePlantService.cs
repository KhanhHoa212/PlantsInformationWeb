using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PlantsInformationWeb.Services
{
    public class FavoritePlantService
    {
        public readonly IGenericRepository<Favoriteplant> _favoriteRepo;

        public readonly IFavoriteRepository _favoriteRepository;
        private readonly IMapper _mapper;


        public FavoritePlantService(IGenericRepository<Favoriteplant> generic, IMapper mapper, IFavoriteRepository favoriteRepository)
        {
            _favoriteRepo = generic;
            _mapper = mapper;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<bool> AddFavoritePlantAsync(FavoritePlantDto favoritePlant)
        {
            var isFav = _favoriteRepo.GetQueryable().Any(f => f.UserId == favoritePlant.UserId && f.PlantId == favoritePlant.PlantId);
            if (isFav) return false;
            var newFav = _mapper.Map<Favoriteplant>(favoritePlant);
            if (newFav.FavoritedAt == default)
            {
                newFav.FavoritedAt = DateTime.Now;
            }
            await _favoriteRepo.AddAsync(newFav);
            return true;
        }
        public async Task<bool> DeleteFavoriteAsync(FavoritePlantDto favoritePlant)
        {
            // Kiểm tra có tồn tại không
            var fav = _favoriteRepo.GetQueryable()
                .FirstOrDefault(f => f.UserId == favoritePlant.UserId && f.PlantId == favoritePlant.PlantId);
            if (fav == null) return false;

            await _favoriteRepo.DeleteAsync(fav.FavoriteId);
            return true;
        }

        public async Task<bool> IsFavoritedAsync(FavoritePlantDto dto)
        {
            var isFav = await Task.FromResult(_favoriteRepo.GetQueryable()
                .Any(f => f.UserId == dto.UserId && f.PlantId == dto.PlantId));
            return isFav;
        }


        public async Task<List<FavoritePlantDto>> GetTopFavoritePlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5)
        {
            return await _favoriteRepository.GetTopFavoritePlantsAsync(startDate, endDate, top);
        }

        public async Task<PaginatedList<PlantSummaryDto>> GetFavoritePlantsPagedAsync(int userId, int pageIndex = 1, int pageSize = 12)
        {
            if (userId <= 0)
            {
                return new PaginatedList<PlantSummaryDto>(new List<PlantSummaryDto>(), 0, pageIndex, pageSize);
            }

            return await _favoriteRepository.GetFavoritePlantsAsync(pageIndex, pageSize, userId);
        }


        public async Task<byte[]> ExportPlantFavoritePdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
        {
            var favorites = await _favoriteRepository.GetTopFavoritePlantsAsync(startDate, endDate, 10);

            byte[] chartImgBytes = null;
            if (!string.IsNullOrWhiteSpace(chartImageBase64))
            {
                var base64 = chartImageBase64.Substring(chartImageBase64.IndexOf(',') + 1);
                chartImgBytes = Convert.FromBase64String(base64);
            }

            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Top Favorite Plants Report")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Column(col =>
                    {
                        if (chartImgBytes != null)
                        {
                            col.Item().Image(chartImgBytes).FitWidth();
                            col.Item().PaddingBottom(10);
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("No.");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Scientific Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Category");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Favorite Count");
                            });

                            int idx = 1;
                            foreach (var plant in favorites)
                            {
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(idx.ToString());
                                table.Cell().Element(e => e.PaddingVertical(5)).Text(plant.PlantName ?? "");
                                table.Cell().Element(e => e.PaddingVertical(5)).Text(plant.ScientificName ?? "");
                                table.Cell().Element(e => e.PaddingVertical(5)).Text(plant.CategoryName ?? "");
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.ViewCount.ToString());
                                idx++;
                            }
                        });

                        col.Item().Element(e => e.PaddingTop(8))
                            .Text($"Total plants: {favorites.Count}")
                            .FontSize(14).Bold().AlignRight();
                    });

                    page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });

            return doc.GeneratePdf();
        }



    }
}