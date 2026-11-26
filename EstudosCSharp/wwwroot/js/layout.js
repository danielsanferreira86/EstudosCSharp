document.addEventListener("DOMContentLoaded", function () {

    const btnMenu =
        document.getElementById("btnMenu");

    const sidebarOverlay =
        document.getElementById("sidebarOverlay");

    const sidebar =
        document.getElementById("appSidebar");

    const body = document.body;

    const telaMobile =
        window.matchMedia("(max-width: 768px)");

    if (!btnMenu || !sidebarOverlay || !sidebar) {
        return;
    }

    function atualizarEstadoMenu() {

        const menuAberto = telaMobile.matches
            ? body.classList.contains("sidebar-open")
            : !body.classList.contains("sidebar-collapsed");

        btnMenu.setAttribute(
            "aria-expanded",
            String(menuAberto));

    }

    function fecharMenuMobile() {

        body.classList.remove("sidebar-open");

        atualizarEstadoMenu();

    }

    btnMenu.addEventListener("click", function () {

        if (telaMobile.matches) {

            body.classList.toggle("sidebar-open");

        } else {

            body.classList.toggle("sidebar-collapsed");

        }

        atualizarEstadoMenu();

    });

    sidebarOverlay.addEventListener(
        "click",
        fecharMenuMobile);

    telaMobile.addEventListener("change", function () {

        body.classList.remove("sidebar-open");

        atualizarEstadoMenu();

    });

    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape" && telaMobile.matches) {

            fecharMenuMobile();

            btnMenu.focus();

        }

    });

    atualizarEstadoMenu();

});