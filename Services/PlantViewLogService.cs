using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Repository;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PlantsInformationWeb.Services
{
    public class PlantViewLogService
    {
        private readonly PlantViewLogRepository _plantViewLogRepository;

        public PlantViewLogService(PlantViewLogRepository plantViewLogRepository)
        {
            _plantViewLogRepository = plantViewLogRepository;
        }

        public async Task<List<PlantViewLogDto>> GetTopViewedPlantsAsync(DateTime? startDate, DateTime? endDate, int top = 5)
        {
            return await _plantViewLogRepository.GetTopViewedPlantsAsync(startDate, endDate, top);
        }

        public async Task<byte[]> ExportPlantViewPdfAsync(DateTime? startDate, DateTime? endDate, string chartImageBase64)
        {
            // Lấy danh sách top viewed plants (phải đảm bảo DTO có ScientificName và CategoryName)
            var plantviews = await _plantViewLogRepository.GetTopViewedPlantsAsync(startDate, endDate, 10);

            // Decode ảnh chart từ base64
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

                    page.Header().Text("Top Viewed Plants Report")
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
                                columns.ConstantColumn(40);    // No.
                                columns.RelativeColumn(3);     // Plant Name
                                columns.RelativeColumn(3);     // Scientific Name
                                columns.RelativeColumn(2);     // Category
                                columns.RelativeColumn(1);     // View Count
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("No.");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Plant Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Scientific Name");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("Category");
                                header.Cell().Element(e => e.BorderBottom(1).PaddingVertical(5).AlignCenter()).Text("View Count");
                            });

                            int idx = 1;
                            foreach (var plant in plantviews)
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
                            .Text($"Total plants: {plantviews.Count}")
                            .FontSize(14).Bold().AlignRight();
                    });

                    page.Footer().AlignCenter().Text($"Exported at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });

            return doc.GeneratePdf();
        }
    }
}