// Chatbox functionality
document.addEventListener("DOMContentLoaded", function () {
  const chatButton = document.getElementById("chatButton");
  const chatWindow = document.getElementById("chatWindow");
  const chatClose = document.getElementById("chatClose");
  const chatForm = document.getElementById("chatForm");
  const chatInput = document.getElementById("chatInput");
  const chatMessages = document.getElementById("chatMessages");

  // Toggle chat window
  function openChat() {
    chatWindow.classList.add("active");
    chatButton.style.display = "none";
    chatInput.focus();
    loadChatHistory();
  }

  function closeChat() {
    chatWindow.classList.remove("active");
    chatButton.style.display = "flex";
  }

  // Event listeners
  chatButton.addEventListener("click", openChat);
  chatClose.addEventListener("click", closeChat);

  chatForm.addEventListener("submit", function (e) {
    e.preventDefault();

    const messageText = chatInput.value.trim();
    if (messageText === "") return;

    chatInput.value = "";

    addMessage(messageText, "user");

    addBotResponse(messageText);
  });

  // Add message to chat
  function addMessage(text, sender) {
    const messageDiv = document.createElement("div");
    messageDiv.className = `message ${sender}-message`;

    const currentTime = new Date().toLocaleTimeString("en-US", {
      hour: "numeric",
      minute: "2-digit",
    });

    if (sender === "bot") {
      messageDiv.innerHTML = `
                <div class="message-avatar">
                    <i class="bi bi-flower2"></i>
                </div>
                <div class="message-content">
                    <div class="message-bubble">${text}</div>
                    <span class="message-time">${currentTime}</span>
                </div>
            `;
    } else {
      messageDiv.innerHTML = `
                <div class="message-content">
                    <div class="message-bubble">${text}</div>
                    <span class="message-time">${currentTime}</span>
                </div>
            `;
    }

    chatMessages.appendChild(messageDiv);

    // Scroll to bottom
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }

  async function createSessionIfNeeded() {
    let sessionId = window.currentSessionId;
    if (sessionId && sessionId > 0) {
      return sessionId;
    }
    return 0;
  }
  function showTypingIndicator() {
    const indicator = document.getElementById("typingIndicator");
    if (indicator) {
      indicator.style.display = "block";
      
      const chatMessages = document.getElementById("chatMessages");
      chatMessages.scrollTop = chatMessages.scrollHeight;
    }
  }

  function hideTypingIndicator() {
    const indicator = document.getElementById("typingIndicator");
    if (indicator) indicator.style.display = "none";
  }

  async function addBotResponse(userMessage) {
    const sessionId = await createSessionIfNeeded();
    console.log("SessionId gửi lên API:", sessionId);
    if (!sessionId || sessionId <= 0) {
      addMessage("Bạn cần đăng nhập để sử dụng chat!", "bot");
      return;
    }
    showTypingIndicator();

    const response = await fetch("/Api/ChatApi?handler=SendMessage", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ sessionId, userInput: userMessage }),
    });

    hideTypingIndicator();

    if (response.ok) {
      const data = await response.json();
      renderMessages(data);
    } else {
      addMessage("Sorry, something went wrong. ❌", "bot");
    }
  }

  function renderMessages(messages) {
    chatMessages.innerHTML = "";
    if (!messages || messages.length === 0) {
      chatMessages.innerHTML = `
      <div class="message bot-message">
        <div class="message-avatar">
          <i class="bi bi-flower2"></i>
        </div>
        <div class="message-content">
          <div class="message-bubble">
            Hello! 👋 I'm your Plant Assistant. How can I help you today?
          </div>
          <span class="message-time">Just now</span>
        </div>
      </div>
    `;
      return;
    }
    messages.forEach((msg) => {
      // Sử dụng đúng key như từ response
      const text = msg.MessageText || "";
      const time = formatTime(msg.SentAt);
      const sender = msg.SenderType;
      const messageDiv = document.createElement("div");
      if (sender === "user") {
        messageDiv.className = "message user-message";
        messageDiv.innerHTML = `
        <div class="message-content">
          <div class="message-bubble">${text}</div>
          <span class="message-time">${time}</span>
        </div>
      `;
      } else {
        messageDiv.className = "message bot-message";
        messageDiv.innerHTML = `
        <div class="message-avatar">
          <i class="bi bi-flower2"></i>
        </div>
        <div class="message-content">
          <div class="message-bubble">${text}</div>
          <span class="message-time">${time}</span>
        </div>
      `;
      }
      chatMessages.appendChild(messageDiv);
    });
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }

  function formatTime(dateString) {
    if (!dateString) return "";
    const d = new Date(dateString);
    return d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  }
  async function loadChatHistory() {
    await createSessionIfNeeded();
    const sessionId = window.currentSessionId;
    const response = await fetch(
      `/Api/ChatApi?handler=History&sessionId=${sessionId}`
    );
    if (response.ok) {
      const data = await response.json();
      renderMessages(data);
    } else {
      renderMessages([]);
    }
  }
});
 