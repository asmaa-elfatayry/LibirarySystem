(function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    applyTheme(savedTheme);

    document.addEventListener('DOMContentLoaded', function () {
        const toggleBtn = document.getElementById('themeToggle');
        const icon = document.getElementById('themeIcon');
        const label = document.getElementById('themeLabel');

        function updateToggleUI(theme) {
            icon.textContent = theme === 'dark' ? '☀️' : '🌙';
            label.textContent = theme === 'dark' ? 'لايت مود' : 'دارك مود';
        }

        updateToggleUI(savedTheme);

        toggleBtn.addEventListener('click', function () {
            const current = document.documentElement.getAttribute('data-theme');
            const newTheme = current === 'dark' ? 'light' : 'dark';
            applyTheme(newTheme);
            localStorage.setItem('theme', newTheme);
            updateToggleUI(newTheme);
        });
    });

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);   // متغيراتنا الخاصة
        document.documentElement.setAttribute('data-bs-theme', theme); // دارك مود Bootstrap الرسمي
    }
})();