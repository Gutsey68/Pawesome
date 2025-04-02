// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const themeToggle = document.getElementById('theme-toggle');

function applyTheme(isDark) {
    if (isDark) {
        document.body.classList.add('dark');
        document.documentElement.classList.add('dark');
    } else {
        document.body.classList.remove('dark');
        document.documentElement.classList.remove('dark');
    }
}

const storedPreference = localStorage.getItem('darkMode');

if (storedPreference === null) {
    const prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
    applyTheme(prefersDarkMode);
    localStorage.setItem('darkMode', prefersDarkMode.toString());
} else {
    const isDarkMode = storedPreference === 'true';
    applyTheme(isDarkMode);
}

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
    if (!localStorage.getItem('userChoseTheme')) {
        const prefersDarkMode = e.matches;
        applyTheme(prefersDarkMode);
        localStorage.setItem('darkMode', prefersDarkMode.toString());
    }
});

themeToggle.addEventListener('click', function() {
    const isDark = document.body.classList.contains('dark');
    applyTheme(!isDark);
    localStorage.setItem('darkMode', (!isDark).toString());
    localStorage.setItem('userChoseTheme', 'true');
});