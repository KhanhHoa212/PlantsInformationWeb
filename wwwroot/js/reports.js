// Dashboard Charts Initialization with Mock Data
function generateColors(n) {
  // Tạo màu dựa trên HSL, đảm bảo đa dạng và hài hòa
  const colors = [];
  for (let i = 0; i < n; i++) {
    colors.push(`hsl(${(360 * i) / n}, 70%, 60%)`);
  }
  return colors;
}
(() => {
  function initializeReports() {
    if (typeof window.jQuery === "undefined" || typeof Chart === "undefined") {
      console.log("[v0] Dependencies not loaded yet, retrying...");
      setTimeout(initializeReports, 100);
      return;
    }

    const $ = window.jQuery;

    $(document).ready(() => {
      console.log("[v0] Initializing Reports page...");

      // Chart.js Global Configuration
      Chart.defaults.font.family =
        "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";
      Chart.defaults.color = "#6b7280";
      Chart.defaults.plugins.legend.display = true;
      Chart.defaults.plugins.legend.position = "bottom";
      Chart.defaults.plugins.legend.labels.padding = 15;
      Chart.defaults.plugins.legend.labels.usePointStyle = true;

      // Color Palette
      const colors = {
        primary: "#16a34a",
        secondary: "#22c55e",
        accent: "#10b981",
        blue: "#3b82f6",
        purple: "#8b5cf6",
        orange: "#f59e0b",
        red: "#ef4444",
        teal: "#14b8a6",
        pink: "#ec4899",
      };

      let currentPage = 1;
      const itemsPerPage = 10;
      let sortColumn = "id";
      let sortDirection = "asc";

      // Initialize Charts
      initializeCharts();

      // Event Listeners
      $("#generateReport").on("click", generateReport);
      $("#applyFilters").on("click", applyFilters);
      $("#resetFilters").on("click", resetFilters);
      $("#exportCSV").on("click", () => exportData("csv"));
      $("#exportExcel").on("click", () => exportData("excel"));
      $("#exportPDF").on("click", () => exportData("pdf"));

      // Sortable columns
      $(".sortable").on("click", function () {
        const column = $(this).data("sort");
        handleSort(column);
      });

      // Initialize Charts Function
      function initializeCharts() {
        // 1. User Registration Line Chart
        const userRegistrationCtx = document.getElementById(
          "userRegistrationChart"
        );
        if (userRegistrationCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();
          fetch(
            `/Admin/ReportsSection?handler=UserAdditionChartData&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((chartData) => {
              new Chart(userRegistrationCtx, {
                type: "line",
                data: {
                  labels: chartData.labels,
                  datasets: [
                    {
                      label: "New Users",
                      data: chartData.data,
                      borderColor: colors.blue,
                      backgroundColor: "rgba(59, 130, 246, 0.1)",
                      borderWidth: 3,
                      fill: true,
                      tension: 0.4,
                      pointRadius: 5,
                      pointHoverRadius: 7,
                      pointBackgroundColor: colors.blue,
                      pointBorderColor: "#ffffff",
                      pointBorderWidth: 2,
                    },
                  ],
                },
                options: {
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  plugins: {
                    legend: {
                      display: false,
                    },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          return `New Users: ${context.parsed.y}`;
                        },
                      },
                    },
                  },
                  scales: {
                    y: {
                      beginAtZero: true,
                      grid: {
                        color: "#f3f4f6",
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                    x: {
                      grid: {
                        display: false,
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                  },
                },
              });
            });
        }
        // Export pdf
        $("#exportUserRegistrationsPdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          // Nếu không chọn, tự động lấy cả năm hiện tại
          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("userRegistrationChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportUserRegistrations",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "user_registration_chart.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
          });
        });

        // 2. Plant Addition Line Chart
        const plantAdditionCtx = document.getElementById("plantAdditionChart");
        if (plantAdditionCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();
          fetch(
            `/Admin/ReportsSection?handler=PlantAdditionChartData&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((chartData) => {
              new Chart(plantAdditionCtx, {
                type: "line",
                data: {
                  labels: chartData.labels,
                  datasets: [
                    {
                      label: "New Plants",
                      data: chartData.data,
                      borderColor: colors.primary,
                      backgroundColor: "rgba(22, 163, 74, 0.1)",
                      borderWidth: 3,
                      fill: true,
                      tension: 0.4,
                      pointRadius: 5,
                      pointHoverRadius: 7,
                      pointBackgroundColor: colors.primary,
                      pointBorderColor: "#ffffff",
                      pointBorderWidth: 2,
                    },
                  ],
                },
                options: {
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  plugins: {
                    legend: {
                      display: false,
                    },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          return `New Plants: ${context.parsed.y}`;
                        },
                      },
                    },
                  },
                  scales: {
                    y: {
                      beginAtZero: true,
                      grid: {
                        color: "#f3f4f6",
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                    x: {
                      grid: {
                        display: false,
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                  },
                },
              });
            });
        }
        // Export pdf
        $("#exportPlantAdditionPdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          // Nếu không chọn, tự động lấy cả năm hiện tại
          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("plantAdditionChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantAddition",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantAdditiont.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
          });
        });

        // 3.Region Bar Chart
        const regionBarCtx = document.getElementById("regionBarChart");
        if (regionBarCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();
          fetch(
            `/Admin/ReportsSection?handler=RegionChartData&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((chartData) => {
              const colors = generateColors(chartData.labels.length);
              new Chart(regionBarCtx, {
                type: "bar",
                data: {
                  labels: chartData.labels,
                  datasets: [
                    {
                      label: "Number of Plants",
                      data: chartData.data,
                      backgroundColor: "#16a34a",
                      borderColor: "#16a34a",
                      borderWidth: 0,
                      borderRadius: 8,
                      hoverBackgroundColor: "#22c55e",
                    },
                  ],
                },
                options: {
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2.5,
                  plugins: {
                    legend: {
                      display: false,
                    },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          return `Plants: ${context.parsed.y}`;
                        },
                      },
                    },
                  },
                  scales: {
                    y: {
                      beginAtZero: true,
                      grid: {
                        color: "#f3f4f6",
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                    x: {
                      grid: {
                        display: false,
                        drawBorder: false,
                      },
                      ticks: {
                        padding: 10,
                      },
                    },
                  },
                },
              });
            });
        }
        $("#exportPlantDistributionPdf").on("click", function () {
          console.log("Export Plant PDF clicked"); // Thêm dòng này để test
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("regionBarChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantDistribution",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantDistribution.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
            error: function () {
              alert("Export PDF failed!");
            },
          });
        });

        // 4. Category Pie Chart
        const categoryPieCtx = document.getElementById("categoryPieChart");
        if (categoryPieCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();
          fetch(
            `/Admin/ReportsSection?handler=CategoryPlantChartData&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((chartData) => {
              const dynamicColors = generateColors(chartData.labels.length);
              new Chart(categoryPieCtx, {
                type: "pie",
                data: {
                  labels: chartData.labels,
                  datasets: [
                    {
                      data: chartData.data,
                      backgroundColor: dynamicColors,
                      borderWidth: 2,
                      borderColor: "#ffffff",
                      hoverOffset: 10,
                    },
                  ],
                },
                options: {
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  plugins: {
                    legend: {
                      position: "right",
                    },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          const label = context.label || "";
                          const value = context.parsed || 0;
                          const total = context.dataset.data.reduce(
                            (a, b) => a + b,
                            0
                          );
                          const percentage = ((value / total) * 100).toFixed(1);
                          return `${label}: ${value} plants (${percentage}%)`;
                        },
                      },
                    },
                  },
                },
              });
            });
        }
        $("#exportPlantDistributionByCategoryPdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("categoryPieChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantDistributionByCategory",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantDistributionByCategory.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
            error: function () {
              alert("Export PDF failed!");
            },
          });
        });

        // 5. Climate Doughnut Chart
        const climateDoughnutCtx = document.getElementById(
          "climateDoughnutChart"
        );
        if (climateDoughnutCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();
          fetch(
            `/Admin/ReportsSection?handler=ClimateChartData&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((chartData) => {
              const colors = generateColors(chartData.labels.length);
              new Chart(climateDoughnutCtx, {
                type: "doughnut",
                data: {
                  labels: chartData.labels,
                  datasets: [
                    {
                      data: chartData.data,
                      backgroundColor: colors,
                      borderWidth: 2,
                      borderColor: "#ffffff",
                      hoverOffset: 10,
                    },
                  ],
                },
                options: {
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  cutout: "65%",
                  plugins: {
                    legend: {
                      position: "right",
                    },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          const label = context.label || "";
                          const value = context.parsed || 0;
                          const total = context.dataset.data.reduce(
                            (a, b) => a + b,
                            0
                          );
                          const percentage = ((value / total) * 100).toFixed(1);
                          return `${label}: ${value} plants (${percentage}%)`;
                        },
                      },
                    },
                  },
                },
              });
            });
        }
        $("#exportPlantDistributionByClimatePdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("climateDoughnutChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantDistributionByClimate",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantDistributionByClimate.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
            error: function () {
              alert("Export PDF failed!");
            },
          });
        });

        // 6. Top Plants Viewed - Horizontal Bar Chart
        const topPlantsCtx = document.getElementById("topPlantsChart");
        if (topPlantsCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();

          fetch(
            `/Admin/ReportsSection?handler=TopViewedPlants&top=5&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((data) => {
              if (!data || data.length === 0) {
                return;
              }
              const labels = data.map((item) => item.PlantName);
              const viewCounts = data.map((item) => item.ViewCount);
              const barColors = generateColors(labels.length);

              new Chart(topPlantsCtx, {
                type: "bar",
                data: {
                  labels: labels,
                  datasets: [
                    {
                      label: "Views",
                      data: viewCounts,
                      backgroundColor: barColors,
                      borderRadius: 8,
                      borderWidth: 1,
                    },
                  ],
                },
                options: {
                  indexAxis: "y",
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  plugins: {
                    legend: { display: false },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          return `Số lượt xem: ${context.parsed.x}`;
                        },
                      },
                    },
                  },
                  scales: {
                    x: {
                      beginAtZero: true,
                      grid: { color: "#f3f4f6", drawBorder: false },
                      ticks: { padding: 10 },
                    },
                    y: {
                      grid: { display: false, drawBorder: false },
                      ticks: { padding: 10 },
                    },
                  },
                },
              });
            })
            .catch((error) => {
              console.error("Lỗi khi lấy Top 5 Plants Viewed:", error);
            });
        }
        $("#exportPlantViewPdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("topPlantsChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantView",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantView.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
            error: function () {
              alert("Export PDF failed!");
            },
          });
        });

         // 7. Top Plants Favorite - Horizontal Bar Chart
        const topPlantsFavCtx = document.getElementById("topPlantsFavoriteChart");
        if (topPlantsFavCtx) {
          const startDate = $("#startDate").val();
          const endDate = $("#endDate").val();

          fetch(
            `/Admin/ReportsSection?handler=TopFavoritePlants&top=5&startDate=${startDate}&endDate=${endDate}`
          )
            .then((response) => response.json())
            .then((data) => {
              if (!data || data.length === 0) {
                // Nếu không có dữ liệu thì có thể hiển thị thông báo ở đây
                return;
              }
              const labels = data.map((item) => item.PlantName);
              const viewCounts = data.map((item) => item.ViewCount);
              const barColors = generateColors(labels.length);

              new Chart(topPlantsFavCtx, {
                type: "bar",
                data: {
                  labels: labels,
                  datasets: [
                    {
                      label: "Views",
                      data: viewCounts,
                      backgroundColor: barColors,
                      borderRadius: 8,
                      borderWidth: 1,
                    },
                  ],
                },
                options: {
                  indexAxis: "y",
                  responsive: true,
                  maintainAspectRatio: true,
                  aspectRatio: 2,
                  plugins: {
                    legend: { display: false },
                    tooltip: {
                      backgroundColor: "rgba(0, 0, 0, 0.8)",
                      padding: 12,
                      cornerRadius: 8,
                      callbacks: {
                        label: function (context) {
                          return `Số lượt yêu thích: ${context.parsed.x}`;
                        },
                      },
                    },
                  },
                  scales: {
                    x: {
                      beginAtZero: true,
                      grid: { color: "#f3f4f6", drawBorder: false },
                      ticks: { padding: 10 },
                    },
                    y: {
                      grid: { display: false, drawBorder: false },
                      ticks: { padding: 10 },
                    },
                  },
                },
              });
            })
            .catch((error) => {
              console.error("Lỗi khi lấy Top 5 Plants Favorite:", error);
            });
        }
        $("#exportPlantFavoritePdf").on("click", function () {
          let startDate = $("#startDate").val();
          let endDate = $("#endDate").val();

          if (!startDate) {
            const now = new Date();
            startDate = `${now.getFullYear()}-01-01`;
          }
          if (!endDate) {
            const now = new Date();
            endDate = `${now.getFullYear()}-12-31`;
          }

          const chartCanvas = document.getElementById("topPlantsFavoriteChart");
          const chartImage = chartCanvas.toDataURL("image/png");
          $.ajax({
            url: "/Admin/ReportsSection?handler=ExportPlantFavorite",
            method: "POST",
            data: {
              startDate: startDate,
              endDate: endDate,
              format: "pdf",
              chartImage: chartImage,
            },
            xhrFields: { responseType: "blob" },
            success: function (data) {
              const url = window.URL.createObjectURL(data);
              const a = document.createElement("a");
              a.href = url;
              a.download = "PlantFavorite.pdf";
              document.body.appendChild(a);
              a.click();
              window.URL.revokeObjectURL(url);
            },
            error: function () {
              alert("Export PDF failed!");
            },
          });
        });
      }

      function generateReport() {
        showAlert("Generating report with current filters...", "info");
        setTimeout(() => {
          showAlert("Report generated successfully!", "success");
        }, 1500);
      }

      function applyFilters() {
        showAlert("Filters applied successfully!", "success");
        // In real implementation, filter the data based on selected filters
      }

      function resetFilters() {
        $("#dataType").val("all");
        $("#exportFormat").val("csv");
        $("#reportCategory").val("");
        $("#reportClimate").val("");
        $("#reportRegion").val("");
        $("#startDate").val("");
        $("#endDate").val("");
        $("#tableSearch").val("");
        currentPage = 1;
        renderPagination();
        showAlert("Filters reset successfully!", "info");
      }

      function exportData(format) {
        showAlert(`Exporting data as ${format.toUpperCase()}...`, "info");
        setTimeout(() => {
          showAlert(
            `Data exported successfully as ${format.toUpperCase()}!`,
            "success"
          );
        }, 1500);
      }

      function showAlert(message, type = "info") {
        const alertHtml = `
                    <div class="alert alert-${type} alert-dismissible fade show position-fixed"
                         style="top: 90px; right: 20px; z-index: 9999; min-width: 300px; border-radius: 12px; 
                                box-shadow: 0 10px 30px rgba(0,0,0,0.15);" role="alert">
                        <i class="bi bi-${
                          type === "success"
                            ? "check-circle"
                            : type === "info"
                            ? "info-circle"
                            : "exclamation-circle"
                        } me-2"></i>
                        ${message}
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                `;
        $("body").append(alertHtml);
        setTimeout(() => $(".alert").alert("close"), 3000);
      }

      console.log("[v0] Reports page initialized successfully");
    });
  }

  initializeReports();
})();
