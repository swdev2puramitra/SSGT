// Dropdown menu hover functionality (Desktop)
document.addEventListener('DOMContentLoaded', function() {
    if (window.innerWidth < 992) return;

    document.querySelectorAll('.nav-item.dropdown').forEach(dropdown => {
        const menu = dropdown.querySelector('.dropdown-menu');
        const toggle = dropdown.querySelector('[data-bs-toggle="dropdown"]');

        if (!menu || !toggle) return;

        dropdown.addEventListener('mouseenter', () => {
            menu.classList.add('show');
            toggle.setAttribute('aria-expanded', 'true');
        });

        dropdown.addEventListener('mouseleave', () => {
            menu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
        });
    });
});


