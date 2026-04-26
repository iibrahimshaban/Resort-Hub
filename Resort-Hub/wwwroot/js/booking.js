(() => {
    "use strict";

    let days = null;
    let picker = null;
    let pricePerNight = Number(document.getElementById("totalPrice").dataset.pricePerNight || 0);

    const dom = {};

    document.addEventListener("DOMContentLoaded", init);

    async function init() {

        cacheDom();
        bindEvents();
        await loadBookedDates();
        updateUI();

        document.getElementById("main-container").classList.remove("d-none");
    }

    async function loadBookedDates()
    {
        try {

            const villaId = document.getElementById("villaId").value;

            const response = await fetch(`/book/booked-dates/${villaId}`);

            if (!response.ok)
                throw new Error("Failed loading dates");

            const data = await response.json();

            initCalendar(data);

        } catch (error) {
            initCalendar([]);
        }
    }

    function cacheDom() {
        dom.daysText = document.getElementById("daysText");
        //dom.daysInput = document.getElementById("daysInput");
        dom.perDays = document.getElementById("perDays");
        dom.totalPrice = document.getElementById("totalPrice");
        dom.dateRange = document.getElementById("dateRange");

        dom.dateStart = document.getElementById("dateStart");
        dom.dateEnd = document.getElementById("dateEnd");

        dom.plusBtn = document.querySelector(".plus-btn");
        dom.minusBtn = document.querySelector(".minus-btn");

        dom.submitBtn = document.getElementById("submit-btn");
    }

    function bindEvents() {
        dom.plusBtn?.addEventListener("click", () => changeDays(1));
        dom.minusBtn?.addEventListener("click", () => changeDays(-1));
    }

    function initCalendar(bookedRanges = []) {

        picker = flatpickr("#dateRange", {
            mode: "range",
            minDate: "today",
            dateFormat: "d M",
            disableMobile: true,
            locale: {
                rangeSeparator: " - "
            },
            disable: bookedRanges.map(x => ({
                from: new Date(x.checkInDate),
                to: new Date(subtractOneDay(x.checkOutDate))
            })),

            onChange(selectedDates) {

                if (selectedDates.length === 2) {

                    const start = selectedDates[0];
                    const end = selectedDates[1];

                    const diff = Math.abs(end - start);

                    days = Math.round(diff / (1000 * 60 * 60 * 24));

                    if (days < 1) days = 1;

                    dom.dateStart.value = this.formatDate(start,"Y-m-d");
                    dom.dateEnd.value = this.formatDate(end, "Y-m-d");

                    updateUI();
                }
                else {
                    days = null;
                    updateUI();
                }
            }
        });
    }

    function subtractOneDay(dateStr) {

        const date = new Date(dateStr);

        date.setDate(date.getDate() - 1);

        return date.toISOString().split("T")[0];
    }

    function changeDays(value) {

        if (days === null)
            return;

        days += value;

        if (days < 1) days = 1;

        syncCalendar();
        updateUI();
    }

    function syncCalendar() {

        if (!picker || picker.selectedDates.length === 0)
            return;

        const start = picker.selectedDates[0];
        const end = new Date(start);

        end.setDate(start.getDate() + days);

        picker.setDate([start, end], true);
    }

    function updateUI() {

        const hasValidDays = days !== null;

        dom.plusBtn.disabled = !hasValidDays;
        dom.minusBtn.disabled = !hasValidDays;
        dom.submitBtn.disabled = !hasValidDays;

        if (!hasValidDays)
        {
            dom.daysText.innerHTML = `<span class="pay-text">Select Date</span>`;
            dom.perDays.innerHTML = ``;
            dom.totalPrice.textContent = `$0 USD`;
            return;
        }

        dom.daysText.textContent =
            `${days} ${days === 1 ? "Day" : "Days"}`;


        dom.perDays.innerHTML =
            `<span class="pay-text">per</span> ${days} ${days === 1 ? "Day" : "Days"}`;

        const total = days * pricePerNight;

        //dom.daysInput.value = days;

        dom.totalPrice.textContent = `$${total} USD`;
    }

})();