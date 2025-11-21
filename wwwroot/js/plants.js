// Plants Page JavaScript

(() => {
  function initializePlantsPage() {
    // Check if jQuery is loaded
    if (typeof window.jQuery === "undefined") {
      console.log("[v0] jQuery not loaded yet, retrying...");
      setTimeout(initializePlantsPage, 100);
      return;
    }

    const $ = window.jQuery;
    const bootstrap = window.bootstrap;
    const tempusDominus = window.tempusDominus;

    $(document).ready(() => {
      console.log("[v0] Initializing Plants page...");

      // Initialize tooltips
      var tooltipTriggerList = [].slice.call(
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
      );
      var tooltipList = tooltipTriggerList.map(
        (tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl)
      );

      // Initialize Tempus Dominus date picker
      if (document.getElementById("plantingSeasonPicker") && tempusDominus) {
        const plantingSeasonPicker = new tempusDominus.TempusDominus(
          document.getElementById("plantingSeasonPicker"),
          {
            display: {
              viewMode: "months",
              components: {
                decades: false,
                year: true,
                month: true,
                date: false,
                hours: false,
                minutes: false,
                seconds: false,
              },
            },
            localization: {
              format: "MMMM",
            },
          }
        );
      }

      // View Toggle (Grid/List)
      $("#gridView, #listView").on("click", function () {
        const $plantGrid = $("#plantGrid");
        const isGridView = $(this).attr("id") === "gridView";

        // Toggle active state
        $("#gridView, #listView").removeClass("active");
        $(this).addClass("active");

        // Toggle view classes
        if (isGridView) {
          $plantGrid
            .removeClass("plant-list-view")
            .addClass("plant-grid-view row g-2");
          $plantGrid
            .find(".plant-item")
            .removeClass("col-12")
            .addClass("col-md-6 col-lg-3");
        } else {
          $plantGrid
            .removeClass("plant-grid-view row g-4")
            .addClass("plant-list-view");
          $plantGrid
            .find(".plant-item")
            .removeClass("col-md-6 col-lg-4")
            .addClass("col-12");
        }

        // Store preference
        localStorage.setItem(
          "plantViewPreference",
          isGridView ? "grid" : "list"
        );
      });

      // Restore view preference
      const viewPreference = localStorage.getItem("plantViewPreference");
      if (viewPreference === "list") {
        $("#listView").trigger("click");
      }

      // Clear Filters
      $("#clearFilters").on("click", function () {
        $(".filter-section-content .form-check-input").prop("checked", false);

        $("#plantingSeasonInput").val("");

        showAlert("All filters cleared", "info");

        console.log("[v0] Filters cleared");

        var form = $("#filterForm");
        form[0].reset(); 

        // Chuyển hướng về trang gốc không có filter
        window.location.href = form.attr("action"); 
      });

      // Collect filter values
      function collectFilters() {
        const filters = {
          categories: [],
          climates: [],
          soils: [],
          season: $("#plantingSeasonInput").val(),
        };

        // Collect category filters
        $("#categoryFilter .form-check-input:checked").each(function () {
          filters.categories.push($(this).val());
        });

        // Collect climate filters
        $("#climateFilter .form-check-input:checked").each(function () {
          filters.climates.push($(this).val());
        });

        // Collect soil filters
        $("#soilFilter .form-check-input:checked").each(function () {
          filters.soils.push($(this).val());
        });

        return filters;
      }

      $(document).ready(function () {
        $("#filterForm input[type=checkbox]").on("change", function () {
          $("#filterForm")[0].submit();
        });
      });

      // Sort functionality
      $("#sortSelect").on("change", function () {
        const sortValue = $(this).val();
        console.log("[v0] Sorting by:", sortValue);

        showAlert(`Sorting by ${sortValue}...`, "info");

        // Here you would typically re-fetch or re-sort the plants
      });

      // Smooth collapse animation for filter sections
      $(".collapse").on("show.bs.collapse", function () {
        var targetId = $(this).attr("id");
        var $btn = $("[data-bs-target='#" + targetId + "']");
        var $icon = $btn.find(".bi-chevron-down");
        $icon.css("transform", "rotate(-90deg)");
      });
      $(".collapse").on("hide.bs.collapse", function () {
        var targetId = $(this).attr("id");
        var $btn = $("[data-bs-target='#" + targetId + "']");
        var $icon = $btn.find(".bi-chevron-down");
        $icon.css("transform", "rotate(0deg)");
      });

      // Alert function
      function showAlert(message, type = "info") {
        const alertHtml = `
                    <div class="alert alert-${type} alert-dismissible fade show position-fixed"
                         style="top: 20px; right: 20px; z-index: 9999; min-width: 300px; border-radius: 12px; 
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

      // Make showAlert globally available
      window.showAlert = showAlert;

      console.log("[v0] Plants page initialized successfully");
    });
  }

  // Start initialization
  initializePlantsPage();
})();
