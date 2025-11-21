// Declare $ and bootstrap variables
const $ = window.jQuery;
const bootstrap = window.bootstrap;

function showNotification(type, message, title = null, duration = 3000) {
  const config = {
    success: {
      bgColor: "#ffffff",
      borderColor: "#10b981",
      iconBgColor: "#d1fae5",
      iconColor: "#10b981",
      textColor: "#1f2937",
      descColor: "#6b7280",
      progressColor: "#10b981",
      icon: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none">
<circle cx="12" cy="12" r="10" fill="currentColor"/>
<path d="M8 12l3 3 5-5" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
</svg>`,
      defaultTitle: "Success",
    },
    error: {
      bgColor: "#ffffff",
      borderColor: "#ef4444",
      iconBgColor: "#fee2e2",
      iconColor: "#ef4444",
      textColor: "#1f2937",
      descColor: "#6b7280",
      progressColor: "#ef4444",
      icon: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none">
<circle cx="12" cy="12" r="10" fill="currentColor"/>
<path d="M8 8l8 8M16 8l-8 8" stroke="white" stroke-width="2" stroke-linecap="round"/>
</svg>`,
      defaultTitle: "Error",
    },
    info: {
      bgColor: "#ffffff",
      borderColor: "#3b82f6",
      iconBgColor: "#dbeafe",
      iconColor: "#3b82f6",
      textColor: "#1f2937",
      descColor: "#6b7280",
      progressColor: "#3b82f6",
      icon: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none">
<circle cx="12" cy="12" r="10" fill="currentColor"/>
<path d="M12 8v.01M12 12v4" stroke="white" stroke-width="2" stroke-linecap="round"/>
</svg>`,
      defaultTitle: "Info",
    },
    warning: {
      bgColor: "#ffffff",
      borderColor: "#f59e0b",
      iconBgColor: "#fef3c7",
      iconColor: "#f59e0b",
      textColor: "#1f2937",
      descColor: "#6b7280",
      progressColor: "#f59e0b",
      icon: `<svg width="24" height="24" viewBox="0 0 24 24" fill="none">
<path d="M12 2l10 18H2L12 2z" fill="currentColor"/>
<path d="M12 9v4M12 15h.01" stroke="white" stroke-width="1.5" stroke-linecap="round"/>
</svg>`,
      defaultTitle: "Warning",
    },
  };

  const style = config[type] || config.info;
  const notificationTitle = title || style.defaultTitle;

  // Container chính
  const toast = document.createElement("div");
  toast.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: ${style.bgColor};
    border-radius: 8px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
    z-index: 9999;
    display: flex;
    align-items: stretch;
    max-width: 400px;
    animation: notificationSlideIn 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
    overflow: hidden;
  `;

  // Thanh bên trái
  const leftBar = document.createElement("div");
  leftBar.style.cssText = `
    width: 5px;
    background: ${style.borderColor};
    border-radius: 8px 0 0 8px;
    flex-shrink: 0;
  `;

  // Container nội dung
  const contentWrapper = document.createElement("div");
  contentWrapper.style.cssText = `
    display: flex;
    gap: 12px;
    padding: 16px;
    flex: 1;
    align-items: flex-start;
  `;

  // Icon container
  const iconContainer = document.createElement("div");
  iconContainer.style.cssText = `
    background: ${style.iconBgColor};
    width: 40px;
    height: 40px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    color: ${style.iconColor};
  `;
  iconContainer.innerHTML = style.icon;

  // Container text (title + description)
  const textContainer = document.createElement("div");
  textContainer.style.cssText = `
    flex: 1;
  `;

  // Title
  const titleEl = document.createElement("div");
  titleEl.textContent = notificationTitle;
  titleEl.style.cssText = `
    font-weight: 600;
    font-size: 15px;
    color: ${style.textColor};
    margin-bottom: 4px;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
  `;

  // Description/Message
  const messageEl = document.createElement("div");
  messageEl.textContent = message;
  messageEl.style.cssText = `
    font-size: 14px;
    color: ${style.descColor};
    line-height: 1.4;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
  `;

  textContainer.appendChild(titleEl);
  textContainer.appendChild(messageEl);

  // Close button
  const closeBtn = document.createElement("button");
  closeBtn.innerHTML = `<svg width="20" height="20" viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.5">
<path d="M15 5L5 15M5 5L15 15"/>
</svg>`;
  closeBtn.style.cssText = `
    background: none;
    border: none;
    color: #9ca3af;
    cursor: pointer;
    padding: 4px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    transition: color 0.2s;
  `;
  closeBtn.onmouseover = () => (closeBtn.style.color = "#6b7280");
  closeBtn.onmouseout = () => (closeBtn.style.color = "#9ca3af");
  closeBtn.onclick = () => {
    toast.style.animation =
      "notificationSlideOut 0.3s cubic-bezier(0.36, 0, 0.66, -0.56)";
    setTimeout(() => toast.remove(), 300);
  };

  // Progress bar
  const progressBar = document.createElement("div");
  progressBar.style.cssText = `
    position: absolute;
    bottom: 0;
    left: 0;
    height: 3px;
    background: ${style.progressColor};
    animation: notificationProgress ${duration}ms linear;
    width: 100%;
  `;

  contentWrapper.appendChild(iconContainer);
  contentWrapper.appendChild(textContainer);
  contentWrapper.appendChild(closeBtn);

  toast.appendChild(leftBar);
  toast.appendChild(contentWrapper);
  toast.appendChild(progressBar);

  // Thêm CSS animation
  if (!document.querySelector("#notification-styles")) {
    const styleSheet = document.createElement("style");
    styleSheet.id = "notification-styles";
    styleSheet.textContent = `
      @keyframes notificationSlideIn {
        from {
          transform: translateX(420px);
          opacity: 0;
        }
        to {
          transform: translateX(0);
          opacity: 1;
        }
      }
 
      @keyframes notificationSlideOut {
        from {
          transform: translateX(0);
          opacity: 1;
        }
        to {
          transform: translateX(420px);
          opacity: 0;
        }
      }
 
      @keyframes notificationProgress {
        from {
          width: 100%;
        }
        to {
          width: 0%;
        }
      }
    `;
    document.head.appendChild(styleSheet);
  }

  document.body.appendChild(toast);

  setTimeout(() => {
    if (toast.parentNode) {
      toast.style.animation =
        "notificationSlideOut 0.3s cubic-bezier(0.36, 0, 0.66, -0.56)";
      setTimeout(() => toast.remove(), 300);
    }
  }, duration);
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
    // loadPlantDetails(plantId);
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
    const tabTriggerEl = document.querySelector(
      `button[data-bs-target="${hash}"]`
    );
    if (tabTriggerEl) {
      const tab = new bootstrap.Tab(tabTriggerEl);
      tab.show();
    }
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
            showNotification("success", "Removed from favorites");
          } else {
            $btn
              .addClass("favorited btn-success")
              .removeClass("btn-outline-success");
            $btn.html('<i class="bi bi-heart-fill"></i> Remove from Favorites');
            showNotification("success", "Added to favorite");
          }
        } else {
          showNotification("danger", "Operation failed!");
        }
      },
      error: () => {
        showNotification("danger", "Server error!");
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
      showNotification("warning", "Please write a comment");
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
      showNotification("warning", "Please write a reply");

      return;
    }
    // Luôn tìm comment-item gốc (không có .reply-item)
    const $rootComment = $(this).closest(".comment-item:not(.reply-item)");
    const parentCommentId = $rootComment.data("comment-id");
    const $replyContainer = $rootComment.find(".replies-container").first();
    submitNewComment(plantId, replyText, parentCommentId, $replyContainer);

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
        const hash = window.location.hash;
        if (hash && hash.startsWith("#comment-")) {
          setTimeout(function () {
            const commentId = hash.replace("#comment-", "");
            scrollToComment(commentId);
          }, 100);
        }
      },
      error: function () {
        showNotification("danger", "Cannot load comments");
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
    <div class="comment-item ${replyClass} mb-3 fade-in"
     id="comment-${comment.CommentId}"
     data-comment-id="${comment.CommentId}">
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
          showNotification(res.message || "danger", "Bình luận không hợp lệ");
          return;
        }
        const comment = res.comment;
        if (!comment.ParentCommentId) {
          $(".comments-list").prepend(buildCommentHtml(comment));
        } else {
          $replyContainer.append(buildCommentHtml(comment, true));
        }
        showNotification("success", "Comment posted successfully!");
      },
      error: function () {
        showNotification("danger", "Không thể gửi comment!");
      },
    });
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
          showNotification("success", "Shared successfully!");
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
        showNotification("success", "Link copied to clipboard!");
      })
      .catch(() => {
        showNotification("error", "Unable to copy link");
      });
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
  showNotification("infor", "Generating PDF...");

  setTimeout(() => {
    showNotification("success", "PDF export completed!");
    $("#exportModal").modal("hide");
    console.log("PDF export would start here");
  }, 2000);
}

function exportToExcel() {
  showNotification("infor", "Generating Exel file...");

  setTimeout(() => {
    showNotification("success", "Excel export completed!");

    $("#exportModal").modal("hide");
    console.log("Excel export would start here");
  }, 2000);
}
