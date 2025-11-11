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
  function initializeDashboard() {
    // Check if jQuery and Chart.js are loaded
    if (typeof window.jQuery === "undefined" || typeof Chart === "undefined") {
      console.log("[v0] Dependencies not loaded yet, retrying...");
      setTimeout(initializeDashboard, 100);
      return;
    }

    const $ = window.jQuery;

    $(document).ready(() => {
      console.log("[v0] Initializing Dashboard charts...");

      // Chart.js Global Configuration
      Chart.defaults.font.family =
        "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";
      Chart.defaults.color = "#6b7280";
      Chart.defaults.plugins.legend.display = true;
      Chart.defaults.plugins.legend.position = "bottom";
      Chart.defaults.plugins.legend.labels.padding = 15;
      Chart.defaults.plugins.legend.labels.usePointStyle = true;

      // Color Palette - Nature Inspired
      const colors = {
        primary: "#16a34a",
        secondary: "#22c55e",
        accent: "#10b981",
        light: "#dcfce7",
        blue: "#3b82f6",
        purple: "#8b5cf6",
        orange: "#f59e0b",
        red: "#ef4444",
        teal: "#14b8a6",
        pink: "#ec4899",
      };

      // 1. Pie Chart - Plants by Category
      const categoryPieCtx = document.getElementById("categoryPieChart");
      if (categoryPieCtx) {
        fetch("/Admin/Dashboard?handler=CategoryPlantChartData")
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

      // 2. Doughnut Chart - Plants by Climate
      const climateDoughnutCtx = document.getElementById(
        "climateDoughnutChart"
      );
      if (climateDoughnutCtx) {
        fetch("/Admin/Dashboard?handler=ClimateChartData")
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

      // 3. Bar Chart - Plants by Region
      const regionBarCtx = document.getElementById("regionBarChart");
      if (regionBarCtx) {
        fetch("/Admin/Dashboard?handler=RegionChartData")
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

      const userRegistrationCtx = document.getElementById(
        "userRegistrationChart"
      );
      if (userRegistrationCtx) {
        fetch("/Admin/Dashboard?handler=UserAdditionChartData")
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

      // 5. Line Chart - New Plants Added Over Time
      const plantAdditionCtx = document.getElementById("plantAdditionChart");
      if (plantAdditionCtx) {
        fetch("/Admin/Dashboard?handler=PlantAdditionChartData")
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

      // Alert function
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

        setTimeout(() => {
          $(".alert").alert("close");
        }, 3000);
      }

      console.log("[v0] Dashboard initialized successfully with all charts");
    });
  }

  // Start initialization
  initializeDashboard();
})();
