using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PlantsInformationWeb.Services
{
    public class RegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IGenericRepository<Region> _regionRepo;


        public RegionService(IRegionRepository regionRepository, IGenericRepository<Region> genericRepository)
        {
            _regionRepository = regionRepository;
            _regionRepo = genericRepository;
        }

        public async Task<(string[] Labels, int[] Data)> GetRegionListsAsync(DateTime startDate, DateTime endDate)
        {
            return await _regionRepository.GetRegionListsAsync(startDate, endDate);
        }

        public async Task<bool> AddRegionAsync(RegionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RegionName) || string.IsNullOrWhiteSpace(model.Description))
            {
                return false;
            }

            var region = new Region
            {
                RegionName = model.RegionName,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            await _regionRepo.AddAsync(region);
            return true;
        }

        public async Task<bool> DeleteRegionAsync(int id)
        {
            var region = await _regionRepo.GetByIdAsync(id);
            if (region == null)
            {
                return false;
            }

            await _regionRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateRegionAsync(RegionViewModel model)
        {
            var region = await _regionRepo.GetByIdAsync(model.RegionId);
            if (region == null)
            {
                return false;
            }

            region.RegionName = model.RegionName;
            region.Description = model.Description;
            region.UpdatedAt = DateTime.Now;

            await _regionRepo.UpdateAsync(region);
            return true;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            var regions = await _regionRepo.GetAllAsync();
            return regions.ToList();
        }

        public IQueryable<RegionDto> GetRegionQuery(string search)
        {
            var query = _regionRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.ToLower();
                query = query.Where(r => r.RegionName.ToLower().Contains(keyword));
            }

            return query
                .Select(r => new RegionDto
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName,
                    Description = r.Description,
                    PlantCount = r.Plants.Count()
                })
                .OrderBy(r => r.RegionId);
        }

        public async Task<int> GetTotalRegionCountAsync()
        {
            return await _regionRepo.GetCountAsync();
        }

        public async Task<(RegionDto TopRegion, double percent)> GetTop1RegionWithPercentAsync()
        {
            var topRegion = await _regionRepository.GetTopRegionAsync();
            var totalPlantCount = await _regionRepository.GetCountAllRegionsAsync();
            double percent = 0;

            if (topRegion != null && totalPlantCount > 0)
            {
                percent = (double)topRegion.PlantCount / totalPlantCount * 100;
            }
            return (topRegion, percent);
        }

        public async Task<byte[]> ExportPlantDistributionPdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
        {
            var regions = await _regionRepository.GetPlantDistributionByRegionAsync(startDate, endDate);

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

                    page.Header().Text("Plant Distribution by Region")
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
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(2); 
                                columns.RelativeColumn(2); 
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("RegionId");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Region Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant numbers");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Created at");
                            });

                            foreach (var region in regions)
                            {
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(region.RegionId.ToString());
                                table.Cell().Element(e => e.PaddingVertical(5)).Text(region.RegionName ?? "");
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(region.PlantCount.ToString());
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text( region.CreatedAt.HasValue ? region.CreatedAt.Value.ToString("dd/MM/yyyy") : "");
                            }
                        });

                        col.Item().Element(e => e.PaddingTop(8))
                            .Text($"Total number of regions: {regions.Count}")
                            .FontSize(14).Bold().AlignRight();
                    });

                    page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });

            return doc.GeneratePdf();
        }

    }
}