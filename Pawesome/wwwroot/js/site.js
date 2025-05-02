/**
 * @fileoverview Theme management module for the Pawesome application
 */

"use strict";

/**
 * Button element that toggles between light and dark themes
 * @type {HTMLElement}
 */
const themeToggle = document.getElementById('theme-toggle');

/**
 * Applies the theme to the document based on user preference
 * @param {boolean} isDark - Whether to apply dark theme (true) or light theme (false)
 */
function applyTheme(isDark) {
    if (isDark) {
        document.body.classList.add('dark');
        document.documentElement.classList.add('dark');
    } else {
        document.body.classList.remove('dark');
        document.documentElement.classList.remove('dark');
    }
}

/**
 * User's stored theme preference from local storage
 * @type {string|null}
 */
const storedPreference = localStorage.getItem('darkMode');

// Apply theme from stored preferences or system preference if not set
if (storedPreference === null) {
    /**
     * System preference for dark mode
     * @type {boolean}
     */
    const prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
    applyTheme(prefersDarkMode);
    localStorage.setItem('darkMode', prefersDarkMode.toString());
} else {
    /**
     * User's preferred theme (dark or light)
     * @type {boolean}
     */
    const isDarkMode = storedPreference === 'true';
    applyTheme(isDarkMode);
}

/**
 * Event listener for system theme preference changes
 * Only applies if user hasn't explicitly chosen a theme
 */
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
    if (!localStorage.getItem('userChoseTheme')) {
        const prefersDarkMode = e.matches;
        applyTheme(prefersDarkMode);
        localStorage.setItem('darkMode', prefersDarkMode.toString());
    }
});

/**
 * Event listener for theme toggle button
 * Switches between light and dark themes and stores the preference
 */
themeToggle.addEventListener('click', function() {
    const isDark = document.body.classList.contains('dark');
    applyTheme(!isDark);
    localStorage.setItem('darkMode', (!isDark).toString());
    localStorage.setItem('userChoseTheme', 'true');
});