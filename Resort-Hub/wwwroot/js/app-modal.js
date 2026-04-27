const AppModal = (() => {

    let modal;
    let modalEl;
    let title;
    let body;
    let footer;
    let header;

    function init() {

        modalEl = document.getElementById("app-modal");
        if (!modalEl) return false;

        modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        title = document.getElementById("modal-title");
        body = document.getElementById("modal-body");
        footer = document.getElementById("modal-footer");
        header = modalEl.querySelector(".modal-header");

        return true;
    }

    function hide() {
        if (modal) modal.hide();
    }

    function show({
        titleText = "Info",
        bodyText = "",
        type = "info",
        buttons = []
    }) {

        if (!modal && !init()) return;

        title.innerText = titleText;
        body.innerHTML = bodyText;
        footer.innerHTML = "";

        applyType(type);

        if (buttons.length === 0) {
            buttons.push({
                text: "Close",
                className: "btn btn-secondary",
                dismiss: true
            });
        }

        buttons.forEach(btnData => {

            const btn = document.createElement("button");
            btn.type = "button";
            btn.innerText = btnData.text;
            btn.className = btnData.className || "btn btn-primary";

            if (btnData.dismiss) {
                btn.setAttribute("data-bs-dismiss", "modal");
            }

            if (btnData.onClick) {
                btn.addEventListener("click", btnData.onClick);
            }

            footer.appendChild(btn);
        });

        modal.show();
    }

    function applyType(type) {

        header.className = "modal-header";

        switch (type.toLowerCase()) {

            case "success":
                header.classList.add("bg-success", "text-white");
                break;

            case "error":
            case "danger":
                header.classList.add("bg-danger", "text-white");
                break;

            case "warning":
                header.classList.add("bg-warning");
                break;

            default:
                header.classList.add("bg-primary", "text-white");
                break;
        }
    }

    return {
        show,
        hide
    };

})();