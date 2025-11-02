using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using PlantsInformationWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace PlantsInformationWeb.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class ReportsSectionModel : PageModel
    {
        private readonly PlantsInformationContext _context;
        public ReportsSectionModel(PlantsInformationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostExportPdfAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Lấy dữ liệu cây trồng (có thể thêm Include nếu cần)
            var plants = await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .ToListAsync();

            // Tạo PDF bằng QuestPDF
            var stream = new MemoryStream();

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text("Plants Information Report")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        // Định nghĩa số cột
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Plant Name").Bold();
                            header.Cell().Element(CellStyle).Text("Scientific Name").Bold();
                            header.Cell().Element(CellStyle).Text("Category").Bold();
                            header.Cell().Element(CellStyle).Text("Climate").Bold();
                            header.Cell().Element(CellStyle).Text("Created At").Bold();
                        });

                        // Rows
                        foreach (var plant in plants)
                        {
                            table.Cell().Element(CellStyle).Text(plant.PlantName);
                            table.Cell().Element(CellStyle).Text(plant.ScientificName);
                            table.Cell().Element(CellStyle).Text(plant.Category?.CategoryName ?? "");
                            table.Cell().Element(CellStyle).Text(plant.Climate?.ClimateName ?? "");
                            table.Cell().Element(CellStyle).Text(plant.CreatedAt?.ToString("yyyy-MM-dd") ?? "");
                        }

                        // Cell style helper
                        IContainer CellStyle(IContainer container)
                            => container.PaddingVertical(5).PaddingHorizontal(2);
                    });

                    page.Footer().AlignCenter().Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            })
            .GeneratePdf(stream);

            stream.Position = 0;
            return File(stream, "application/pdf", "PlantsReport.pdf");
        }

        public async Task<IActionResult> OnPostExportCsvAsync()
        {
            var plants = await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Climate)
                .ToListAsync();

            var csv = new StringBuilder();

            // Header: Thêm Plant ID vào đầu
            csv.AppendLine("Plant ID,Plant Name,Scientific Name,Category,Climate,Created At,Status");

            foreach (var plant in plants)
            {
                string plantId = $"\"{plant.PlantId}\"";
                string plantName = $"\"{plant.PlantName}\"";
                string scientificName = $"\"{plant.ScientificName}\"";
                string category = $"\"{plant.Category?.CategoryName ?? ""}\"";
                string climate = $"\"{plant.Climate?.ClimateName ?? ""}\"";
                string createdAt = $"\"{plant.CreatedAt?.ToString("yyyy-MM-dd") ?? ""}\"";
                string status = $"\"{plant.Status}\"";

                csv.AppendLine($"{plantId},{plantName},{scientificName},{category},{climate},{createdAt},{status}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "PlantsReport.csv");
        }
    }
}