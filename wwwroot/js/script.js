const $ = window.jQuery
const bootstrap = window.bootstrap
const tempusDominus = window.tempusDominus

$(document).ready(() => {
  // Initialize tooltips
  var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
  var tooltipList = tooltipTriggerList.map((tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl))

  // Initialize Tempus Dominus date picker (nếu dùng)
  const plantingSeasonPicker = new tempusDominus.TempusDominus(document.getElementById("plantingSeasonPicker"), {
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
  })

  // Hàm thông báo (nếu vẫn dùng)
  function showAlert(message, type = "info") {
    const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show position-fixed" 
                 style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `
    $("body").append(alertHtml)
    setTimeout(() => {
      $(".alert").alert("close")
    }, 3000)
  }
})