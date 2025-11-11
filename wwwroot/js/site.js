document.addEventListener("DOMContentLoaded", function () {
    const userButton = document.getElementById("userButton");
    const userDropdown = document.getElementById("userDropdown");
 
    if (typeof Popper === 'undefined') {
        console.error("Popper.js chưa được load.");
        return;
    }
 
    if (userButton && userDropdown) {
        const popperInstance = Popper.createPopper(userButton, userDropdown, {
            placement: 'bottom-end',
            modifiers: [
                {
                    name: 'offset',
                    options: {
                        offset: [0, 8],
                    },
                },
                {
                    name: 'preventOverflow',
                    options: {
                        boundary: 'viewport',
                    },
                }
            ],
        });
 
        let isOpen = false;
 
        userButton.addEventListener("click", () => {
            isOpen = !isOpen;
            userDropdown.style.display = isOpen ? "block" : "none";
            if (isOpen) {
                popperInstance.update();
            }
        });
 
        document.addEventListener("click", (event) => {
            if (!userButton.contains(event.target) && !userDropdown.contains(event.target)) {
                userDropdown.style.display = "none";
                isOpen = false;
            }
        });
    }
});
 