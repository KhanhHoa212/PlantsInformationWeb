// Declare $ and bootstrap variables
const $ = window.jQuery;
const bootstrap = window.bootstrap;

function showAlert(message, type = "info") {
  const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show position-fixed"
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

  $("body").append(alertHtml);

  setTimeout(() => {
    $(".alert").alert("close");
  }, 3000);
}

$(document).ready(() => {
  // Initialize tooltips
  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(
    (tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl)
  );

  // Get plant ID from URL
  const urlParams = new URLSearchParams(window.location.search);
  const plantId = urlParams.get("id");

  if (plantId) {
    loadPlantDetails(plantId);
    loadComments(plantId);
  }

  // Tab switching with URL hash
  $('button[data-bs-toggle="tab"]').on("shown.bs.tab", (e) => {
    const tabId = $(e.target).attr("data-bs-target");
    window.location.hash = tabId;
  });

  // Load tab from URL hash
  if (window.location.hash) {
    const hash = window.location.hash;
    $(`button[data-bs-target="${hash}"]`).tab("show");
  }

  // Favorite button functionality
  $("#favorite-btn").on("click", function () {
    var $btn = $(this);
    var plantId = $btn.data("plant-id");
    var isFavorited = $btn.hasClass("favorited");
    var url = isFavorited ? "?handler=RemoveFavorite" : "?handler=AddFavorite";

    $btn.prop("disabled", true);

    $.ajax({
      url: url,
      method: "POST",
      data: { plantId: plantId },
      success: (res) => {
        if (res.success) {
          if (isFavorited) {
            $btn
              .removeClass("favorited btn-success")
              .addClass("btn-outline-success");
            $btn.html('<i class="bi bi-heart"></i> Add to Favorites');
            showAlert("Removed from favorites", "info");
          } else {
            $btn
              .addClass("favorited btn-success")
              .removeClass("btn-outline-success");
            $btn.html('<i class="bi bi-heart-fill"></i> Remove from Favorites');
            showAlert("Added to favorites!", "success");
          }
        } else {
          showAlert("Operation failed!", "danger");
        }
      },
      error: () => {
        showAlert("Server error!", "danger");
      },
      complete: () => {
        $btn.prop("disabled", false);
      },
    });
  });

  // Share button functionality
  $('.btn:contains("Share")').on("click", () => {
    sharePlant();
  });

  // Character counter for new comment
  $("#commentText").on("input", function () {
    const charCount = $(this).val().length;
    $("#charCount").text(charCount);
  });

  // Submit new comment
  $("#newCommentForm").on("submit", function (e) {
    e.preventDefault();
    const commentText = $("#commentText").val().trim();
    if (commentText.length === 0) {
      showAlert("Please write a comment", "warning");
      return;
    }
    submitNewComment(plantId, commentText, null);
    $("#commentText").val("");
    $("#charCount").text("0");
  });

  // Reply button functionality
  $(document).on("click", ".reply-btn", function () {
    // Đóng tất cả reply-form
    $(".reply-form-container").addClass("d-none");
    // Mở đúng reply-form của comment vừa click
    const $commentItem = $(this).closest(".comment-item");
    const $replyForm = $commentItem.find(".reply-form-container").first();
    $replyForm.toggleClass("d-none");
    if (!$replyForm.hasClass("d-none")) {
      const userName = $commentItem.find("h6").first().text();
      const $textarea = $replyForm.find("textarea");
      // Luôn thêm tag @Tên người dùng: nếu chưa có
      if ($textarea.val().indexOf("@" + userName) !== 0) {
        $textarea.val("@" + userName + ": ");
      }
      $textarea.focus();
    }
  });

  // Cancel reply button
  $(document).on("submit", ".reply-form", function (e) {
    e.preventDefault();
    const replyText = $(this).find("textarea").val().trim();
    if (replyText.length === 0) {
      showAlert("Please write a reply", "warning");
      return;
    }
    const $commentItem = $(this).closest(".comment-item");
    const parentCommentId = $commentItem.data("comment-id");
    const isParentReply = $commentItem.hasClass("reply-item");

    let $replyContainer;
    if (isParentReply) {
      // Nếu đang reply cho reply, chuyển về comment gốc
      const $rootComment = $commentItem.closest(
        ".comment-item:not(.reply-item)"
      );
      $replyContainer = $rootComment.find(".replies-container").first();
      submitNewComment(
        plantId,
        replyText,
        $rootComment.data("comment-id"),
        $replyContainer
      );
    } else {
      // reply cho comment gốc
      $replyContainer = $commentItem.find(".replies-container").first();
      submitNewComment(plantId, replyText, parentCommentId, $replyContainer);
    }

    $(this).closest(".reply-form-container").addClass("d-none");
    this.reset();
  });

  // Load commnet with plantId
  function loadComments(plantId) {
    $.ajax({
      url: `/PlantDetail?handler=Comments&id=${plantId}`,
      method: "GET",
      success: function (comments) {
        renderComments(comments);
      },
      error: function () {
        showAlert("Cannot load comments", "danger");
      },
    });
  }
  function renderComments(comments) {
    const $commentsList = $(".comments-list");
    $commentsList.empty();

    comments.forEach((comment) => {
      const html = buildCommentHtml(comment);
      $commentsList.append(html);
    });
  }
  function formatTimeAgo(isoDateString) {
    if (!isoDateString) return "";
    const now = new Date();
    const date = new Date(isoDateString);
    const diff = Math.floor((now - date) / 1000);

    if (diff < 60) return "just now";
    if (diff < 3600) return Math.floor(diff / 60) + " minutes ago";
    if (diff < 86400) return Math.floor(diff / 3600) + " hours ago";
    return date.toLocaleDateString();
  }

  function buildCommentHtml(comment, isReply = false) {
    const replyClass = isReply ? "reply-item" : "";

    // Match mọi tên (cả tiếng Việt, ký tự đặc biệt) cho đến dấu :
    let commentText = comment.CommentText;
    const mentionRegex = /^@([^:]+):\s*/;
    if (isReply && mentionRegex.test(commentText)) {
      const mention = commentText.match(mentionRegex)[1];
      commentText = commentText.replace(
        mentionRegex,
        `<span class="reply-mention text-primary fw-bold">@${mention}:</span> `
      );
    }

    let html = `
    <div class="comment-item ${replyClass} mb-3 fade-in" data-comment-id="${
      comment.CommentId
    }">
      <div class="d-flex gap-3">
        <div class="comment-avatar flex-shrink-0">
          <div class="avatar-circle bg-success text-white d-flex align-items-center justify-content-center fw-bold" title="${
            comment.UserName
          }">
            ${getInitials(comment.UserName)}
          </div>
        </div>
        <div class="comment-content flex-grow-1">
          <div class="comment-header d-flex justify-content-between align-items-start mb-2">
            <div>
              <h6 class="mb-0 fw-bold text-dark">${comment.UserName}</h6>
              <small class="text-muted">
                <i class="bi bi-clock"></i> ${formatTimeAgo(comment.CreatedAt)}
              </small>
            </div>
          </div>
          <p class="comment-text mb-2 text-dark">${commentText}</p>
          <div class="comment-footer">
            <button class="btn btn-sm btn-link text-success p-0 reply-btn" data-comment-id="${
              comment.CommentId
            }">
              <i class="bi bi-reply"></i> Reply
            </button>
          </div>
          <div class="reply-form-container mt-3 d-none">
            <form class="reply-form">
              <div class="input-group">
                <textarea class="form-control" placeholder="Write a reply..." rows="2" maxlength="500" required></textarea>
                <button class="btn btn-success" type="submit">
                  <i class="bi bi-send"></i>
                </button>
                <button class="btn btn-outline-secondary cancel-reply-btn" type="button">
                  <i class="bi bi-x"></i>
                </button>
              </div>
            </form>
          </div>
          <div class="replies-container mt-4 ps-4 border-start border-success border-opacity-25">
            ${
              comment.Replies && comment.Replies.length > 0
                ? comment.Replies.map((r) => buildCommentHtml(r, true)).join("")
                : ""
            }
          </div>
        </div>
      </div>
    </div>
  `;
    return html;
  }

  function getInitials(name) {
    return name
      .split(" ")
      .map((w) => w[0])
      .join("")
      .toUpperCase();
  }
  function submitNewComment(
    plantId,
    commentText,
    parentCommentId = null,
    $replyContainer = null
  ) {
    $.ajax({
      url: "/PlantDetail?handler=AddComment",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        PlantId: plantId,
        CommentText: commentText,
        ParentCommentId: parentCommentId,
      }),
      success: function (res) {
        if (!res.success) {
          showAlert(res.message || "Bình luận không hợp lệ", "danger");
          return;
        }
        const comment = res.comment;
        if (!comment.ParentCommentId) {
          $(".comments-list").prepend(buildCommentHtml(comment));
        } else {
          $replyContainer.append(buildCommentHtml(comment, true));
        }
        showAlert("Comment posted successfully!", "success");
      },
      error: function () {
        showAlert("Không thể gửi comment!", "danger");
      },
    });
  }

  function loadPlantDetails(plantId) {
    showLoadingState();

    setTimeout(() => {
      hideLoadingState();
      console.log(`Loading plant details for ID: ${plantId}`);
    }, 1000);
  }

  function sharePlant() {
    if (navigator.share) {
      navigator
        .share({
          title: document.title,
          text: "Check out this plant information",
          url: window.location.href,
        })
        .then(() => {
          showAlert("Shared successfully!", "success");
        })
        .catch((error) => {
          console.log("Error sharing:", error);
          fallbackShare();
        });
    } else {
      fallbackShare();
    }
  }

  function fallbackShare() {
    navigator.clipboard
      .writeText(window.location.href)
      .then(() => {
        showAlert("Link copied to clipboard!", "success");
      })
      .catch(() => {
        showAlert("Unable to copy link", "error");
      });
  }

  function showLoadingState() {
    $(".plant-header, .tab-content").addClass("loading");
  }

  function hideLoadingState() {
    $(".plant-header, .tab-content").removeClass("loading");
  }

  // Smooth scrolling within tabs
  $(".nav-tabs .nav-link").on("click", () => {
    setTimeout(() => {
      $("html, body").animate(
        {
          scrollTop: $("#plantTabs").offset().top - 100,
        },
        300
      );
    }, 150);
  });

  // Image zoom functionality for carousel
  $(".carousel-item img").on("click", function () {
    const imgSrc = $(this).attr("src");
    const imgAlt = $(this).attr("alt");

    const modal = `
            <div class="modal fade" id="imageModal" tabindex="-1">
                <div class="modal-dialog modal-lg modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">${imgAlt}</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body text-center">
                            <img src="${imgSrc}" class="img-fluid" alt="${imgAlt}">
                        </div>
                    </div>
                </div>
            </div>
        `;

    $("body").append(modal);
    $("#imageModal").modal("show");

    $("#imageModal").on("hidden.bs.modal", function () {
      $(this).remove();
    });
  });

  function createCommentElement(data) {
    const replyClass = data.isReply ? "reply-item" : "";
    const html = `
      <div class="comment-item ${replyClass} mb-3 fade-in" data-comment-id="${
      data.id
    }">
        <div class="d-flex gap-3">
          <div class="comment-avatar flex-shrink-0">
            <div class="avatar-circle ${
              data.userColor
            } text-white d-flex align-items-center justify-content-center fw-bold" title="${
      data.userName
    }">
              ${data.userInitials}
            </div>
          </div>
          <div class="comment-content flex-grow-1">
            <div class="comment-header d-flex justify-content-between align-items-start mb-2">
              <div>
                <h6 class="mb-0 fw-bold text-dark">${data.userName}</h6>
                <small class="text-muted">
                  <i class="bi bi-clock"></i> ${formatTimeAgo(
                    comment.CreatedAt
                  )}
                </small>
              </div>
            </div>
            <p class="comment-text mb-2 text-dark">${data.text}</p>
            <div class="comment-footer">
              <button class="btn btn-sm btn-link text-success p-0 reply-btn" data-comment-id="${
                data.id
              }">
                <i class="bi bi-reply"></i> Reply
              </button>
            </div>
            <div class="reply-form-container mt-3 d-none">
              <form class="reply-form">
                <div class="input-group">
                  <textarea class="form-control" placeholder="Write a reply..." rows="2" maxlength="500" required></textarea>
                  <button class="btn btn-success" type="submit">
                    <i class="bi bi-send"></i>
                  </button>
                  <button class="btn btn-outline-secondary cancel-reply-btn" type="button">
                    <i class="bi bi-x"></i>
                  </button>
                </div>
              </form>
            </div>
            ${
              !data.isReply
                ? '<div class="replies-container mt-4 ps-4 border-start border-success border-opacity-25"></div>'
                : ""
            }
          </div>
        </div>
      </div>
    `;
    return html;
  }
});

// Export functions (called from modal)
function exportToPDF() {
  showAlert("Generating PDF...", "info");

  setTimeout(() => {
    showAlert("PDF export completed!", "success");
    $("#exportModal").modal("hide");
    console.log("PDF export would start here");
  }, 2000);
}

function exportToExcel() {
  showAlert("Generating Excel file...", "info");

  setTimeout(() => {
    showAlert("Excel export completed!", "success");
    $("#exportModal").modal("hide");
    console.log("Excel export would start here");
  }, 2000);
}
