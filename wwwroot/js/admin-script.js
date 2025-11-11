// Additional utility functions
function toggleSidebar() {
  const sidebar = document.querySelector(".sidebar");
  sidebar.classList.toggle("collapsed");
}

function showNotification(type, message) {
  const colorMap = {
    success: "#198754",
    error: "#dc3545",
    warning: "#ffc107",
    info: "#0dcaf0",
  };

  const color = colorMap[type] || "#6c757d";

  const toast = document.createElement("div");
  toast.style.position = "fixed";
  toast.style.top = "20px";
  toast.style.right = "20px";
  toast.style.backgroundColor = color;
  toast.style.color = "#fff";
  toast.style.padding = "12px 20px";
  toast.style.borderRadius = "6px";
  toast.style.boxShadow = "0 2px 6px rgba(0,0,0,0.2)";
  toast.style.zIndex = "9999";
  toast.innerText = message;

  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 3000);
}

function setupImagePreview(textareaId, previewId) {
  const textarea = document.getElementById(textareaId);
  const preview = document.getElementById(previewId);
  if (!textarea || !preview) return;

  function updatePreview() {
    preview.innerHTML = "";
    const urls = textarea.value
      .split("\n")
      .map((s) => s.trim())
      .filter(Boolean);
    urls.forEach((url) => {
      const img = document.createElement("img");
      img.src = url;
      img.className = "img-thumbnail m-2";
      img.style.maxWidth = "120px";
      img.style.maxHeight = "90px";
      img.onerror = function () {
        img.style.border = "2px solid red";
        img.title = "Ảnh lỗi hoặc không tồn tại!";
      };
      preview.appendChild(img);
    });
  }

  textarea.addEventListener("input", updatePreview);

  updatePreview();
}

// Plant Form Multi-Step Functionality
class PlantFormWizard {
  constructor(formId, prefix, reviewId, imageInputSelector, previewSelector) {
    this.formId = formId;
    this.formElement = document.getElementById(formId);
    this.currentStep = 1;
    this.totalSteps = 4;
    this.prefix = prefix;
    this.reviewId = reviewId;
    this.imageInputSelector = imageInputSelector;
    this.previewSelector = previewSelector;
    this.init();
  }

  init() {
    this.bindEvents();
    this.updateStepDisplay();
  }

  bindEvents() {
    // Next buttons
    const nextBtns = this.formElement.querySelectorAll(".nextBtn");
    nextBtns.forEach((nextBtn) => {
      nextBtn.addEventListener("click", () => {
        if (this.validateCurrentStep()) {
          this.nextStep();
        }
      });
    });

    // Previous buttons
    const prevBtns = this.formElement.querySelectorAll(".prevBtn");
    prevBtns.forEach((prevBtn) => {
      prevBtn.addEventListener("click", () => {
        this.prevStep();
      });
    });

    // Modal reset on close
    const modalId =
      this.formId === "addPlantForm" ? "addPlantModal" : "editPlantModal";
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
      modalElement.addEventListener("hidden.bs.modal", () => {
        this.resetForm();
      });
    }
  }

  validateCurrentStep() {
    const currentStepElement = this.formElement.querySelector(
      `.step-content[data-step="${this.currentStep}"]`
    );
    const requiredFields = currentStepElement.querySelectorAll("[required]");
    let isValid = true;
    requiredFields.forEach((field) => {
      if (!field.value.trim()) {
        field.classList.add("is-invalid");
        isValid = false;
      } else {
        field.classList.remove("is-invalid");
      }
    });
    if (!isValid) {
      showNotification("warning", "Please fill in all required fields.");
    }
    return isValid;
  }

  nextStep() {
    if (this.currentStep < this.totalSteps) {
      this.currentStep++;
      this.updateStepDisplay();
      if (this.currentStep === this.totalSteps) {
        this.collectFormData();
        this.populateReview();
      }
    }
  }
  prevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.updateStepDisplay();
    }
  }

  updateStepDisplay() {
    // Ẩn tất cả nội dung step
    this.formElement.querySelectorAll(".step-content").forEach((step) => {
      step.classList.remove("active");
    });

    // Hiện step hiện tại
    const stepContent = this.formElement.querySelector(
      `.step-content[data-step="${this.currentStep}"]`
    );
    if (stepContent) {
      stepContent.classList.add("active");
    }

    // Cập nhật trạng thái progress steps
    this.formElement.querySelectorAll(".step").forEach((step, index) => {
      const stepNumber = index + 1;
      step.classList.remove("active", "completed");
      if (stepNumber === this.currentStep) {
        step.classList.add("active");
      } else if (stepNumber < this.currentStep) {
        step.classList.add("completed");
      }
    });

    // Xử lý nút Previous
    const prevBtns = this.formElement.querySelectorAll(".prevBtn");
    prevBtns.forEach((prevBtn) => {
      prevBtn.style.display = this.currentStep === 1 ? "none" : "inline-block";
    });

    // Xử lý nút Next
    const nextBtns = this.formElement.querySelectorAll(".nextBtn");
    nextBtns.forEach((nextBtn) => {
      nextBtn.style.display =
        this.currentStep === this.totalSteps ? "none" : "inline-block";
    });

    // Xử lý nút Submit
    const submitBtns = this.formElement.querySelectorAll(".submitBtn");
    submitBtns.forEach((submitBtn) => {
      submitBtn.style.display =
        this.currentStep === this.totalSteps ? "inline-block" : "none";
    });
  }
  collectFormData() {
    this.formData = {};

    // Lấy các trường text
    this.formData.plantName = this.formElement.querySelector(
      `[name="${this.prefix}PlantName"]`
    ).value;
    this.formData.scientificName = this.formElement.querySelector(
      `[name="${this.prefix}ScientificName"]`
    ).value;

    // Lấy text của option được chọn
    const categorySelect = this.formElement.querySelector(
      `[name="${this.prefix}CategoryId"]`
    );
    this.formData.category = categorySelect
      ? categorySelect.options[categorySelect.selectedIndex].text
      : "";

    const climateSelect = this.formElement.querySelector(
      `[name="${this.prefix}ClimateId"]`
    );
    this.formData.climate = climateSelect
      ? climateSelect.options[climateSelect.selectedIndex].text
      : "";

    this.formData.growthCycle = this.formElement.querySelector(
      `[name="${this.prefix}GrowthCycle"]`
    ).value;

    // Lấy các checkbox: lấy text của label đi kèm
    this.formData.soilTypes = Array.from(
      this.formElement.querySelectorAll(
        `input[name="${this.prefix}SoilTypeIds"]:checked`
      )
    ).map((cb) => cb.nextElementSibling.textContent);

    this.formData.regions = Array.from(
      this.formElement.querySelectorAll(
        `input[name="${this.prefix}RegionIds"]:checked`
      )
    ).map((cb) => cb.nextElementSibling.textContent);

    this.formData.diseases = Array.from(
      this.formElement.querySelectorAll(
        `input[name="${this.prefix}DiseaseIds"]:checked`
      )
    ).map((cb) => cb.nextElementSibling.textContent);

    const imageUrlsInput = this.formElement.querySelector(
      this.imageInputSelector
    );
    if (imageUrlsInput) {
      this.formData.imageUrls = imageUrlsInput.value
        .split("\n")
        .map((s) => s.trim())
        .filter(Boolean);
    } else {
      this.formData.imageUrls = [];
    }
  }

  populateReview() {
    const reviewContent = document.getElementById(this.reviewId);
    if (!reviewContent) {
      console.error("Không tìm thấy vùng review với id:", this.reviewId);
      return;
    }

    const createReviewItem = (label, value) => `
    <div class="review-item">
      <span class="review-label">${label}:</span>
      <span class="review-value">${value || "Not specified"}</span>
    </div>
  `;

    const createListItem = (label, list) => {
      if (!list || list.length === 0) {
        return createReviewItem(label, "Not specified");
      }

      const itemsHtml = list
        .map((item) => `<li class="review-list-item">${item}</li>`)
        .join("");

      return `
      <div class="review-item">
        <span class="review-label">${label}:</span>
        <ul class="review-list">${itemsHtml}</ul>
      </div>
    `;
    };

    let html = `
    ${createReviewItem("Plant Name", this.formData.plantName)}
    ${createReviewItem("Scientific Name", this.formData.scientificName)}
    ${createReviewItem("Category", this.formData.category)}
    ${createReviewItem("Climate", this.formData.climate)}
    ${createReviewItem("Growth Cycle", this.formData.growthCycle)}
    ${createListItem("Soil Types", this.formData.soilTypes)}
    ${createListItem("Regions", this.formData.regions)}
    ${createListItem("Common Diseases", this.formData.diseases)}
  `;

    reviewContent.innerHTML = html;

    // Hiển thị preview các ảnh
    if (this.formData.imageUrls && this.formData.imageUrls.length > 0) {
      const imageHtml = this.formData.imageUrls
        .map(
          (url) =>
            `<img src="${url}" class="img-thumbnail m-2" style="max-width:80px;max-height:60px;" 
      onerror="this.style.border='2px solid red';this.title='Ảnh lỗi!';" />`
        )
        .join("");
      html += `
    <div class="review-item">
      <span class="review-label">Images:</span>
      <div>${imageHtml}</div>
    </div>
  `;
    } else {
      html += `
    <div class="review-item">
      <span class="review-label">Images:</span>
      <span class="review-value">Not specified</span>
    </div>
  `;
    }
  }

  previewImage(src) {
    const preview = this.formElement.querySelector(".imagePreview");
    const img = preview ? preview.querySelector("img") : null;
    if (img) {
      if (src) {
        img.src = src;
        preview.style.display = "block";
      } else {
        preview.style.display = "none";
      }
    }
  }

  submitForm() {
    if (!this.validateCurrentStep()) {
      return;
    }
    const formData = new FormData(this.formElement);
    // ... giữ nguyên logic ...
    // Close modal after a delay
    const modalId =
      this.formId === "addPlantForm" ? "addPlantModal" : "editPlantModal";
    setTimeout(() => {
      const modal = window.bootstrap.Modal.getInstance(
        document.getElementById(modalId)
      );
      if (modal) modal.hide();
    }, 1500);
  }

  resetForm() {
    this.currentStep = 1;
    this.formElement.reset();
    this.formElement.querySelectorAll(".is-invalid").forEach((field) => {
      field.classList.remove("is-invalid");
    });
    const imagePreview = this.formElement.querySelector("#imagePreview");
    if (imagePreview) imagePreview.style.display = "none";
    this.updateStepDisplay();
  }
}

document.addEventListener("DOMContentLoaded", function () {
  if (document.getElementById("addPlantForm")) {
    new PlantFormWizard("addPlantForm", "AddPlant.", "reviewContentAdd");
  }
  if (document.getElementById("editPlantForm")) {
    new PlantFormWizard("editPlantForm", "EditPlant.", "reviewContentEdit");
  }

  const editModal = document.getElementById("editPlantModal");
  if (editModal) {
    editModal.addEventListener("show.bs.modal", function (event) {
      const btn = event.relatedTarget;
      if (!btn) return;
      const imageUrls = btn.getAttribute("data-image-urls") || "";
      const arrImageUrls = imageUrls ? imageUrls.split(",") : [];
      const textarea = document.getElementById("imageUrlsInputEdit");
      if (textarea) {
        textarea.value = arrImageUrls.join("\n");
        textarea.dispatchEvent(new Event("input"));
      }
    });
  }

  const typeMeta = document.querySelector("meta[name='notify-type']");
  const messageMeta = document.querySelector("meta[name='notify-message']");
  if (typeMeta && messageMeta) {
    const type = typeMeta.getAttribute("content");
    const message = messageMeta.getAttribute("content");
    showNotification(type, message);
  }

  setupImagePreview("imageUrlsInputAdd", "multiImagePreviewAdd");
  setupImagePreview("imageUrlsInputEdit", "multiImagePreviewEdit");
});
