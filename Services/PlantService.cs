using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.Utils;
using PlantsInformationWeb.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

public class PlantService
{
    private readonly IPlantRepository _plantRepository;

    private readonly IGenericRepository<Plant> _plantRepo;
    private readonly IGenericRepository<Disease> _diseaseRepo;
    private readonly IGenericRepository<Soiltype> _soilRepo;
    private readonly IGenericRepository<Region> _regionRepo;
    private readonly IGenericRepository<Unrecognizedplant> _unrecogRepo;
    private readonly IGenericRepository<Plantviewlog> _plantViewLog;
    // private readonly AIService _aiService;
    private readonly IMapper _mapper;

    private readonly ILogger<PlantService> _logger;

    public PlantService(ILogger<PlantService> logger,IGenericRepository<Unrecognizedplant> unrecognized, IGenericRepository<Plantviewlog> plantViewLogRepo, IGenericRepository<Region> regionRepo, IGenericRepository<Soiltype> soilRepo, IPlantRepository plantRepository, IGenericRepository<Plant> plantRepo, IMapper mapper, IGenericRepository<Disease> diseaseRepo)
    {
        _plantRepository = plantRepository;
        _plantRepo = plantRepo;
        _mapper = mapper;
        _diseaseRepo = diseaseRepo;
        _soilRepo = soilRepo;
        _regionRepo = regionRepo;
        _logger = logger;
        _plantViewLog = plantViewLogRepo;
        // _aiService = aIService;
        _unrecogRepo = unrecognized;
    }

    public async Task<int> GetTotalPlantsAsync()
    {
        return await _plantRepository.GetCountAllPLantsAsync();
    }

    public async Task<List<Plant>> GetAllAsync()
    {
        var regions = await _plantRepo.GetAllAsync();
        return regions.ToList();
    }

    public async Task AddAsync(Plant plant)
    {
        await _plantRepo.AddAsync(plant);
    }

    public async Task<PlantViewModel> GetPlantById(int id)
    {
        var plant = await _plantRepository.GetPlantByIdWithDetailsAsync(id);
        if (plant == null) return null;
        return _mapper.Map<PlantViewModel>(plant);
    }

    public async Task AddPlantAsync(AddPlantRequestDto addPlantRequestDto)
    {
        var plant = _mapper.Map<Plant>(addPlantRequestDto);

        plant.CreatedAt = DateTime.Now;
        plant.Status = "Active";

        // Gán các liên kết nếu có
        if (addPlantRequestDto.DiseaseIds != null && addPlantRequestDto.DiseaseIds.Count > 0)
        {
            plant.Diseases = await _diseaseRepo.GetAllByIdAsync(addPlantRequestDto.DiseaseIds);
        }

        if (addPlantRequestDto.RegionIds != null && addPlantRequestDto.RegionIds.Count > 0)
        {
            plant.Regions = await _regionRepo.GetAllByIdAsync(addPlantRequestDto.RegionIds);
        }

        if (addPlantRequestDto.SoilTypeIds != null && addPlantRequestDto.SoilTypeIds.Count > 0)
        {
            plant.Soils = await _soilRepo.GetAllByIdAsync(addPlantRequestDto.SoilTypeIds);

        }
        if (addPlantRequestDto.ImageUrls != null && addPlantRequestDto.ImageUrls.Count > 0)
        {
            plant.Plantimages = addPlantRequestDto.ImageUrls
                .Select(url => new Plantimage
                {
                    ImageUrl = url,
                    IsPrimary = false
                }).ToList();
        }


        await _plantRepo.AddAsync(plant);
    }

    public async Task<bool> DeletePLantAsync(int id)
    {
        var plant = await _plantRepo.GetByIdAsync(id);
        if (plant == null) return false;

        await _plantRepo.DeleteAsync(id);
        return true;
    }

    public async Task<bool> EditPlantAsync(EditPlantRequestDto editPlantRequestDto)
    {

        var plant = await _plantRepository.GetPlantByIdWithDetailsAsync(editPlantRequestDto.PlantId);
        if (plant == null)
        {
            _logger.LogWarning($"Không tìm thấy cây với PlantId = {editPlantRequestDto.PlantId}");
            return false;
        }

        _mapper.Map(editPlantRequestDto, plant);
        plant.UpdatedAt = DateTime.Now;

        if (editPlantRequestDto.DiseaseIds != null)
        {
            var diseases = await _diseaseRepo.GetAllByIdAsync(editPlantRequestDto.DiseaseIds);
            plant.Diseases.Clear();
            foreach (var d in diseases)
                plant.Diseases.Add(d);
        }
        else
        {
            plant.Diseases.Clear();
        }

        if (editPlantRequestDto.RegionIds != null)
        {
            var regions = await _regionRepo.GetAllByIdAsync(editPlantRequestDto.RegionIds);
            plant.Regions.Clear();
            foreach (var r in regions)
                plant.Regions.Add(r);
        }
        else
        {
            plant.Regions.Clear();
        }

        if (editPlantRequestDto.SoilTypeIds != null)
        {
            var soils = await _soilRepo.GetAllByIdAsync(editPlantRequestDto.SoilTypeIds);
            plant.Soils.Clear();
            foreach (var s in soils)
                plant.Soils.Add(s);
        }
        else
        {
            plant.Soils.Clear();
        }
        if (editPlantRequestDto.ImageUrls != null)
        {
            await _plantRepository.RemoveAllImagesOfPlantAsync(plant.PlantId);
            foreach (var url in editPlantRequestDto.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                plant.Plantimages.Add(new Plantimage
                {
                    ImageUrl = url,
                    IsPrimary = false
                });
            }
        }
        await _plantRepo.UpdateAsync(plant);
        return true;
    }


    public IQueryable<Plant> GetPlantQuery(string search, List<int>? categoryIds, List<int>? climateIds, List<int>? soilIds)
    {
        var query = _plantRepo.GetQueryableWithIncludes(p => p.Soils, p => p.Category, p => p.Climate);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.ToLower();
            query = query.Where(d =>
                d.PlantName.ToLower().Contains(keyword) ||
                d.ScientificName.ToLower().Contains(keyword)
            );
        }

        if (categoryIds != null && categoryIds.Any())
        {
            query = query.Where(d => d.CategoryId.HasValue && categoryIds.Contains(d.CategoryId.Value));
        }
        if (climateIds != null && climateIds.Any())
        {
            query = query.Where(d => d.ClimateId.HasValue && climateIds.Contains(d.ClimateId.Value));
        }
        if (soilIds != null && soilIds.Any())
        {
            // Giả sử Plant có ICollection<Soiltype> Soils
            query = query.Where(d => d.Soils.Any(s => soilIds.Contains(s.SoilId)));
        }
        query = query.OrderBy(d => d.CreatedAt);

        return query;
    }

    public async Task<List<PlantSummaryDto>> GetHotPlantsAsync(int topN)
    {
        var hotPlantIds = await _plantRepository.GetHotPlantIdsAsync(topN);

        return await _plantRepository.GetPlantSummariesByIdsAsync(hotPlantIds);
    }

    public async Task<PaginatedList<PlantSummaryDto>> GetPagedPlantsAsync(int pageIndex, int pageSize)
    {
        return await _plantRepository.GetPlantSummariesAsync(pageIndex, pageSize);
    }

    public async Task<PaginatedList<UnrecognizedplantDto>> GetUnrecognizePlants(int pageIndex, int pageSize)
    {
        var query = _unrecogRepo.GetQueryable()
            .ProjectTo<UnrecognizedplantDto>(_mapper.ConfigurationProvider)
            .OrderBy(p => p.UnrecognizedId);

        return await PaginatedList<UnrecognizedplantDto>.CreateAsync(query, pageIndex, pageSize);
    }

    public async Task<(string[] Labels, int[] Data)> GetPlantAdditionByMonthAsync(DateTime startDate, DateTime endDate)
    {
        return await _plantRepository.GetPlantAddByMonthAsync(startDate, endDate);
    }



    public async Task<PaginatedList<PlantSummaryDto>> SearchPlantsPagedAsync(string keyword, int pageIndex, int pageSize, List<int> categoryIds = null, List<int> climateIds = null, List<int> soilIds = null)
    {
        var query = GetPlantQuery(keyword, categoryIds, climateIds, soilIds).ProjectTo<PlantSummaryDto>(_mapper.ConfigurationProvider).OrderBy(d => d.PlantId);
        return await PaginatedList<PlantSummaryDto>.CreateAsync(query, pageIndex, pageSize);
    }

    public async Task LogPlantSearchAsync(int? userId, string keyword, List<int> categoryIds, List<int> climateIds, List<int> soilIds, List<int> plantIds)
    {
        var filterObj = new
        {
            categoryIds,
            climateIds,
            soilIds
        };

        int searchId = await _plantRepository.LogSearchPlantAsync(userId, keyword, filterObj);

        await _plantRepository.LogSearchPlantResultsAsync(searchId, plantIds);
    }

    public async Task IncreasePlantViewAsync(int plantId, int? userId)
    {
        var log = new Plantviewlog
        {
            PlantId = plantId,
            UserId = userId,
            ViewedAt = DateTime.Now
        };

        await _plantViewLog.AddAsync(log);
    }

    public async Task<byte[]> ExportPlantAdditionPdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
    {
        var plants = await _plantRepository.GetPlantAdditionByMonthAsync(startDate, endDate);

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

                page.Header().Text("Danh sách cây được thêm mới")
                    .FontSize(20).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    // Hiển thị biểu đồ nếu có
                    if (chartImgBytes != null)
                    {
                        col.Item().Image(chartImgBytes).FitWidth();
                        col.Item().PaddingBottom(10);
                    }

                    // Bảng cây
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Tên cây
                            columns.RelativeColumn(2); // Tên khoa học
                            columns.RelativeColumn(2); // Nguồn gốc
                            columns.RelativeColumn(2); // Loại cây
                            columns.RelativeColumn(2); // Chu kỳ sinh trưởng
                            columns.RelativeColumn(2); // Ngày thêm
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant Name");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Scientific Name");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Orgin");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Category Name");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Growth Cycle");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Created at");
                        });

                        foreach (var plant in plants)
                        {
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.PlantName ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.ScientificName ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.Origin ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.CategoryName ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(plant.GrowthCycle ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(
                                plant.CreatedAt.HasValue ? plant.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "");
                        }
                    });

                    // Tổng số
                    col.Item().Element(e => e.PaddingTop(8))
                        .Text($"Total number of new plant added: {plants.Count}")
                        .FontSize(14).Bold().AlignRight();
                });

                page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            });
        });

        return doc.GeneratePdf();
    }

}