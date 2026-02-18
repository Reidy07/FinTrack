// FinTrack UI helpers (modals, small interactions)
(() => {
    const openModal = (id) => {
        const el = document.getElementById(id);
        if (!el) return;
        el.classList.remove('hidden');
        el.setAttribute('aria-hidden', 'false');
        const focusable = el.querySelector('input, select, textarea, button, a[href]');
        focusable?.focus?.();
    };

    const closeModal = (id) => {
        const el = document.getElementById(id);
        if (!el) return;
        el.classList.add('hidden');
        el.setAttribute('aria-hidden', 'true');
    };

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
