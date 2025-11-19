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
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PlantsInformationWeb.Services
{
    public class ClimateService
    {
        private readonly IClimateRepository _climateRepository;
        private readonly IGenericRepository<Climate> _climateRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Plant> _plantRepo;


        public ClimateService(IMapper mapper, IClimateRepository climateRepository, IGenericRepository<Plant> plantRepo, IGenericRepository<Climate> genericRepository)
        {
            _climateRepository = climateRepository;
            _climateRepo = genericRepository;
            _mapper = mapper;
            _plantRepo = plantRepo;

        }

        public async Task<List<Climate>> GetClimatesAsync()
        {
            return await _climateRepository.GetClimatetypesAsync();
        }

        public async Task<(string[] Labels, int[] Data)> GetClimatesWithPlantCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _climateRepository.GetClimateToChartWithPlantCountAsync(startDate, endDate);
        }

        public async Task<bool> AddClimateAsync(ClimateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ClimateName) || string.IsNullOrWhiteSpace(model.Description) || string.IsNullOrWhiteSpace(model.HumidityRange))
            {
                return false;
            }
            var climate = new Climate
            {
                ClimateName = model.ClimateName,
                HumidityRange = model.HumidityRange,
                RainfallRange = model.RainfallRange,
                TemperatureRange = model.TemperatureRange,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };
            await _climateRepo.AddAsync(climate);
            return true;
        }

        public async Task<bool> DeleteClimateAsync(int id)
        {
            var climate = await _climateRepo.GetByIdAsync(id);
            if (climate == null)
            {
                return false;
            }
            await _climateRepo.DeleteAsync(id);
            return true;
        }

        public async Task<List<String>> GetLinkedPlantByClimate(int climateId)
        {
            var plants = await _plantRepo.GetAllAsync();
            var linkedPlants = plants
                .Where(p => p.ClimateId == climateId)
                .Select(p => p.PlantName)
                .ToList();
            return linkedPlants;
        }

        public async Task<bool> UpdateClimateAsync(ClimateViewModel model)
        {
            var climate = await _climateRepo.GetByIdAsync(model.ClimateId);
            if (climate == null)
            {
                return false;
            }
            climate.ClimateName = model.ClimateName;
            climate.HumidityRange = model.HumidityRange;
            climate.RainfallRange = model.RainfallRange;
            climate.TemperatureRange = model.TemperatureRange;
            climate.Description = model.Description;
            climate.UpdatedAt = DateTime.Now;

            await _climateRepo.UpdateAsync(climate);
            return true;
        }

        public async Task<List<Climate>> GetAllAsync()
        {
            var climates = await _climateRepo.GetAllAsync();
            return climates.ToList();
        }

        public IQueryable<ClimateDto> GetClimateQuery(string search)
        {
            var query = _climateRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.ToLower();
                query = query.Where(r => r.ClimateName.ToLower().Contains(keyword));
            }

            return query.ProjectTo<ClimateDto>(_mapper.ConfigurationProvider).OrderBy(c => c.ClimateId);
        }

        public async Task<byte[]> ExportPlantDistributionByClimatePdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
        {
            var climates = await _climateRepository.GetPlantDistributionByClimateAsync(startDate, endDate);

            byte[] chartImgBytes = null;
            if (!string.IsNullOrWhiteSpace(chartImageBase64) && chartImageBase64.Contains(","))
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

                    page.Header().Text("Phân bố cây trồng theo vùng khí hậu")
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
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("ClimateId");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Climate Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant count");
                            });

                            foreach (var c in climates)
                            {
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(c.ClimateId.ToString());
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(c.ClimateName ?? "");
                                table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(c.PlantCount.ToString());
                            }
                        });

                        col.Item().Element(e => e.PaddingTop(8))
                            .Text($"Tổng số vùng khí hậu: {climates.Count}")
                            .FontSize(14).Bold().AlignRight();
                    });

                    page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });

            return doc.GeneratePdf();
        }
    }
}