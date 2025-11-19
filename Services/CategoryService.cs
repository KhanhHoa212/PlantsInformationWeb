using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.Repository;
using PlantsInformationWeb.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenericRepository<Plantcategory> _categoryRepo;
    private readonly IGenericRepository<Plant> _plantRepo;
    private readonly IMapper _mapper;


    public CategoryService(IMapper mapper, ICategoryRepository categoryRepository, IGenericRepository<Plantcategory> genericRepository, IGenericRepository<Plant> plantRepo)
    {
        _categoryRepository = categoryRepository;
        _plantRepo = plantRepo;
        _categoryRepo = genericRepository;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<int> GetTotalPlantsAsync()
    {
        return await _categoryRepository.GetCountAllCategoryAsync();
    }

    public async Task<List<CategoryWithCountDto>> GetCategoriesWithPlantCountAsync(DateTime? startDate, DateTime? endDate)
    {
        return await _categoryRepository.GetCategoriesWithPlantCountAsync(startDate, endDate);
    }

    public async Task<bool> AddCategoryAsync(CategoryViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.CategoryName) || string.IsNullOrWhiteSpace(model.Description))
            return false;

        var category = new Plantcategory
        {
            CategoryName = model.CategoryName,
            Description = model.Description,
            CreatedAt = DateTime.Now
        };

        await _categoryRepo.AddAsync(category);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        await _categoryRepo.DeleteAsync(id);
        return true;
    }

    public async Task<bool> UpdateCategoryAsync(CategoryViewModel model)
    {
        var category = await _categoryRepo.GetByIdAsync(model.CategoryId);
        if (category == null)
            return false;

        category.CategoryName = model.CategoryName;
        category.Description = model.Description;
        category.UpdatedAt = DateTime.Now;

        await _categoryRepo.UpdateAsync(category);
        return true;
    }

    public async Task<List<Plantcategory>> GetAllAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();
        return categories.ToList();
    }

    public IQueryable<CategoryWithCountDto> GetCategoryQuery(string search)
    {
        var query = _categoryRepo.GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.ToLower();
            query = query.Where(c => c.CategoryName.ToLower().Contains(keyword));

        }

        return query
        .Select(c => new CategoryWithCountDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            PlantCount = c.Plants.Count()
        })
        .OrderBy(c => c.CategoryId);

    }

    public async Task<(CategoryDto TopCategory, double percent)> GetTop1CategoryWithPercentAsync()
    {
        var topCategory = await _categoryRepository.GetTop1CategoryAsync();
        var totalPlantCount = await _categoryRepository.GetCountAllCategoryAsync();
        double percent = 0;

        if (topCategory != null && totalPlantCount > 0)
        {
            percent = (double)topCategory.PlantCount / totalPlantCount * 100;
        }
        return (topCategory, percent);

    }

    public async Task<List<String>> GetLinkedPlantByCategory(int categoryId)
    {
        //d
        var plants = await _plantRepo.GetAllAsync();
        var linkedPlants = plants
            .Where(p => p.CategoryId == categoryId)
            .Select(p => p.PlantName)
            .ToList();
        return linkedPlants;    
    }

    public async Task<byte[]> ExportPlantDistributionByCategoryPdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
    {
        var categories = await _categoryRepository.GetPlantDistributionByCategoryAsync(startDate, endDate);

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

                page.Header().Text("Phân bố cây trồng theo loại")
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
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("CategoryId");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Category Name");
                            header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant Count");
                        });

                        foreach (var cat in categories)
                        {
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(cat.CategoryId.ToString());
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(cat.CategoryName ?? "");
                            table.Cell().Element(e => e.PaddingVertical(5).AlignCenter()).Text(cat.PlantCount.ToString());
                        }
                    });

                    col.Item().Element(e => e.PaddingTop(8))
                        .Text($"Tổng số loại: {categories.Count}")
                        .FontSize(14).Bold().AlignRight();
                });

                page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            });
        });

        return doc.GeneratePdf();
    }
}