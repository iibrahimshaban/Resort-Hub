document.addEventListener("DOMContentLoaded", () => {
    "use strict";

    const input = document.getElementById("icon-input");
    const results = document.getElementById("icon-results");


    input.addEventListener("input", async function () {

        let q = this.value.trim();

        if (q.length < 2) {
            results.style.display = "none";
            return;
        }

        try {

            results.innerHTML = `
                    <div class="p-3 text-center text-muted">
                        Loading...
                    </div>`;
            results.style.display = "block";


            let res = await fetch(`/Amenity/SearchIcons?q=${encodeURIComponent(q)}`);
            let data = await res.json();

            results.innerHTML = "";
            results.style.display = "none";


            if (!data || data.length === 0) {
                results.innerHTML = `
                    <div class="p-3 text-center text-muted">
                        No icons found
                    </div>`;
                results.style.display = "block";
                return;
            }

            data.forEach(icon => {

                let item = document.createElement("div");
                item.className = "p-2 icon-row d-flex align-items-center gap-2";
                item.style.cursor = "pointer";

                item.innerHTML = `
                    <i class="${icon.prefix} fa-${icon.id}"></i>

                    <div class="d-flex flex-column">
                        <span>${icon.label}</span>
                        <small class="text-muted">${icon.id}</small>
                    </div>
                `;

                item.onclick = () => {
                    input.value = icon.label;
                    previewIcon(icon);
                    results.style.display = "none";
                };

                results.appendChild(item);
            });

            results.style.display = "block";

        } catch (err){}
    });


    function previewIcon(val) {
        const el = document.getElementById('icon-preview');
        el.className = `${val.prefix} fa-${val.id}`;
        el.style.fontSize = '1.8rem';
        el.style.color = '#003DFC';
    }
});