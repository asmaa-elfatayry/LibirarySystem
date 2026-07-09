(function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    applyTheme(savedTheme);

    document.addEventListener('DOMContentLoaded', function () {
        const toggleBtn = document.getElementById('themeToggle');
        const icon = document.getElementById('themeIcon');
        const label = document.getElementById('themeLabel');

        // Check if elements exist before using them
        if (!toggleBtn || !icon || !label) {
            console.warn('Theme toggle elements not found');
            return;
        }

        function updateToggleUI(theme) {
            if (theme === 'dark') {
                icon.textContent = '☀️';
                label.textContent = 'لايت مود';
            } else {
                icon.textContent = '🌙';
                label.textContent = 'دارك مود';
            }

            // Animate icon
            icon.style.transform = 'rotate(0deg)';
            icon.style.transition = 'transform 0.6s ease-in-out';
            setTimeout(() => {
                icon.style.transform = 'rotate(360deg)';
            }, 10);
        }

        updateToggleUI(savedTheme);

        toggleBtn.addEventListener('click', function (e) {
            const current = document.documentElement.getAttribute('data-theme');
            const newTheme = current === 'dark' ? 'light' : 'dark';
            applyTheme(newTheme);
            localStorage.setItem('theme', newTheme);
            updateToggleUI(newTheme);

            // Add ripple effect
            createRipple(e);
        });

        // Ripple effect on click
        function createRipple(e) {
            const btn = e.target.closest('.theme-toggle');
            if (!btn) return;

            const ripple = document.createElement('span');
            const rect = btn.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');

            btn.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        }
    });

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
    }
})();
