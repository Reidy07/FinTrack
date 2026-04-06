// FinTrack UI helpers (modals, small interactions)
(() => {
    function openModal(id) {
        const el = document.getElementById(id);
        if (!el) return;

        el.classList.remove('hidden');
        el.setAttribute('aria-hidden', 'false');
    }

    function closeModal(id) {
        const el = document.getElementById(id);
        if (!el) return;

        el.classList.add('hidden');
        el.setAttribute('aria-hidden', 'true');
    }
    document.addEventListener('click', (e) => {
        const openBtn = e.target.closest('[data-modal-open]');
        if (openBtn) {
            const id = openBtn.getAttribute('data-modal-open');
            if (id) openModal(id);
            return;
        }

        const closeBtn = e.target.closest('[data-modal-close]');
        if (closeBtn) {
            const id = closeBtn.getAttribute('data-modal-close');
            if (id) closeModal(id);
            return;
        }

        // Click en backdrop cierra modal
        const backdrop = e.target.classList?.contains('ft-modal-backdrop') ? e.target : null;
        if (backdrop && backdrop.id) closeModal(backdrop.id);
    });

    document.addEventListener('keydown', (e) => {
        if (e.key !== 'Escape') return;
        const open = document.querySelector('.ft-modal-backdrop:not(.hidden)');
        if (open?.id) closeModal(open.id);
    });
})();

// =============================================
// SIDEBAR BEHAVIOR - FinTrack
// =============================================

(function () {
    function initSidebar() {
        const sidebar = document.querySelector(".ft-sidebar");
        const overlay = document.querySelector(".ft-sidebar-overlay");
        const toggleBtn = document.querySelector(".ft-sidebar-toggle");
        const closeBtn = document.querySelector(".ft-sidebar-close");

        console.log("Sidebar init:", {
            sidebar,
            overlay,
            toggleBtn,
            closeBtn
        });

        if (!sidebar || !toggleBtn) {
            console.warn("No se encontró sidebar o toggle.");
            return;
        }

        function openSidebar() {
            sidebar.classList.add("open");
            if (overlay) overlay.classList.add("active");
            document.body.classList.add("no-scroll");
            console.log("openSidebar");
        }

        function closeSidebar() {
            sidebar.classList.remove("open");
            if (overlay) overlay.classList.remove("active");
            document.body.classList.remove("no-scroll");
            console.log("closeSidebar");
        }

        toggleBtn.addEventListener("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            if (sidebar.classList.contains("open")) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });

        if (closeBtn) {
            closeBtn.addEventListener("click", function (e) {
                e.preventDefault();
                closeSidebar();
            });
        }

        if (overlay) {
            overlay.addEventListener("click", function () {
                closeSidebar();
            });
        }

        sidebar.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function () {
                closeSidebar();
            });
        });

        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") {
                closeSidebar();
            }
        });

        closeSidebar();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initSidebar);
    } else {
        initSidebar();
    }
})();