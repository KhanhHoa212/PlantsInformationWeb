
document.addEventListener("DOMContentLoaded", () => {
  const userButton = document.getElementById("userButton")
  const userDropdown = document.getElementById("userDropdown")

  if (typeof Popper === "undefined") {
    console.error("Popper.js chưa được load.")
    return
  }

  if (userButton && userDropdown) {
    const popperInstance = Popper.createPopper(userButton, userDropdown, {
      placement: "bottom-end",
      modifiers: [
        {
          name: "offset",
          options: {
            offset: [0, 12],
          },
        },
        {
          name: "preventOverflow",
          options: {
            boundary: "viewport",
            padding: 8,
          },
        },
      ],
    })

    let isOpen = false

    userButton.addEventListener("click", (e) => {
      e.stopPropagation()
      isOpen = !isOpen
      userDropdown.style.display = isOpen ? "block" : "none"
      userButton.classList.toggle("active", isOpen)
      if (isOpen) {
        popperInstance.update()
      }
    })

    document.addEventListener("click", (event) => {
      if (!userButton.contains(event.target) && !userDropdown.contains(event.target)) {
        userDropdown.style.display = "none"
        userButton.classList.remove("active")
        isOpen = false
      }
    })

    // Close dropdown when clicking on items
    userDropdown.querySelectorAll(".dropdown-item").forEach((item) => {
      item.addEventListener("click", () => {
        userDropdown.style.display = "none"
        userButton.classList.remove("active")
        isOpen = false
      })
    })
  }
})
