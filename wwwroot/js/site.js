
(() => {
    const widgets = document.querySelectorAll("[data-airport-autocomplete]");

    widgets.forEach((widget) => {
        const input = widget.querySelector("[data-airport-input]");
        const dropdown = widget.querySelector("[data-airport-dropdown]");

        if (!input || !dropdown) {
            return;
        }

        let abortController = null;
        let items = [];
        let activeIndex = -1;

        const closeDropdown = () => {
            dropdown.hidden = true;
            dropdown.innerHTML = "";
            items = [];
            activeIndex = -1;
        };

        const applySelection = (item) => {
            input.value = `${item.city} (${item.code})`;
            closeDropdown();
        };

        const renderItems = (results) => {
            items = results;
            activeIndex = -1;

            if (!results.length) {
                dropdown.hidden = false;
                dropdown.innerHTML = "<div class=\"airport-autocomplete__empty\">Нічого не знайдено. Спробуйте інше місто або код аеропорту.</div>";
                return;
            }

            dropdown.hidden = false;
            dropdown.innerHTML = results
                .map((item, index) => `
                    <button type="button" class="airport-autocomplete__item" data-airport-option data-index="${index}">
                        <span class="airport-autocomplete__city">${item.city}</span>
                        <span class="airport-autocomplete__code">${item.code}</span>
                        <span class="airport-autocomplete__name">${item.name}</span>
                    </button>`)
                .join("");
        };

        const highlightActive = () => {
            const options = dropdown.querySelectorAll("[data-airport-option]");
            options.forEach((option, index) => {
                option.classList.toggle("is-active", index === activeIndex);
            });
        };

        const fetchAirports = async (term) => {
            if (abortController) {
                abortController.abort();
            }

            abortController = new AbortController();

            try {
                const endpoint = input.dataset.airportEndpoint;
                const response = await fetch(`${endpoint}?term=${encodeURIComponent(term)}`, {
                    signal: abortController.signal
                });

                if (!response.ok) {
                    closeDropdown();
                    return;
                }

                const results = await response.json();
                renderItems(results);
            } catch (error) {
                if (error.name !== "AbortError") {
                    closeDropdown();
                }
            }
        };

        input.addEventListener("input", () => {
            const term = input.value.trim();

            if (term.length < 2) {
                closeDropdown();
                return;
            }

            fetchAirports(term);
        });

        input.addEventListener("keydown", (event) => {
            if (dropdown.hidden || !items.length) {
                return;
            }

            if (event.key === "ArrowDown") {
                event.preventDefault();
                activeIndex = (activeIndex + 1) % items.length;
                highlightActive();
                return;
            }

            if (event.key === "ArrowUp") {
                event.preventDefault();
                activeIndex = activeIndex <= 0 ? items.length - 1 : activeIndex - 1;
                highlightActive();
                return;
            }

            if (event.key === "Enter" && activeIndex >= 0) {
                event.preventDefault();
                applySelection(items[activeIndex]);
                return;
            }

            if (event.key === "Escape") {
                closeDropdown();
            }
        });

        dropdown.addEventListener("mousedown", (event) => {
            const option = event.target.closest("[data-airport-option]");
            if (!option) {
                return;
            }

            event.preventDefault();
            const index = Number(option.dataset.index);
            if (!Number.isNaN(index) && items[index]) {
                applySelection(items[index]);
            }
        });

        input.addEventListener("blur", () => {
            window.setTimeout(closeDropdown, 120);
        });

        input.addEventListener("focus", () => {
            if (items.length) {
                dropdown.hidden = false;
            }
        });
    });
})();
