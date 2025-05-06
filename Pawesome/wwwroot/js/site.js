/**
 * @fileoverview Theme management module for the Pawesome application.
 * Handles theme toggling, persistence, and system preference sync.
 */

"use strict";

/**
 * Returns all theme toggle elements in the DOM.
 * @returns {NodeListOf<HTMLInputElement|HTMLElement>}
 */
function getThemeToggles() {
    return document.querySelectorAll('#theme-toggle');
}

/**
 * Applies the selected theme to the document.
 * @param {boolean} isDark - If true, applies dark theme; otherwise, light theme.
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
 * Updates all theme toggles to reflect the current theme.
 * @param {boolean} isDark - If true, toggles are set to dark mode.
 */
function syncThemeToggles(isDark) {
    getThemeToggles().forEach(toggle => {
        if (toggle.type === 'checkbox') {
            toggle.checked = isDark;
        }
    });
}

/**
 * Initializes the theme based on local storage or system preference.
 */
function initializeTheme() {
    const storedPreference = localStorage.getItem('darkMode');
    let isDark;
    if (storedPreference === null) {
        isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        localStorage.setItem('darkMode', isDark.toString());
    } else {
        isDark = storedPreference === 'true';
    }
    applyTheme(isDark);
    syncThemeToggles(isDark);
}

/**
 * Handles system theme preference changes.
 */
function handleSystemThemeChange() {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        if (!localStorage.getItem('userChoseTheme')) {
            const prefersDark = e.matches;
            applyTheme(prefersDark);
            syncThemeToggles(prefersDark);
            localStorage.setItem('darkMode', prefersDark.toString());
        }
    });
}

/**
 * Handles user interaction with theme toggles.
 */
function handleThemeToggleEvents() {
    getThemeToggles().forEach(toggle => {
        if (toggle.type === 'checkbox') {
            toggle.addEventListener('change', function () {
                const isDark = this.checked;
                applyTheme(isDark);
                localStorage.setItem('darkMode', isDark.toString());
                localStorage.setItem('userChoseTheme', 'true');
                syncThemeToggles(isDark);
            });
        } else {
            // For button (icon) toggles
            toggle.addEventListener('click', function () {
                const isDark = document.body.classList.contains('dark');
                applyTheme(!isDark);
                localStorage.setItem('darkMode', (!isDark).toString());
                localStorage.setItem('userChoseTheme', 'true');
                syncThemeToggles(!isDark);
            });
        }
    });
}

/**
 * Observes theme changes and keeps toggles in sync.
 */
function observeThemeClass() {
    const observer = new MutationObserver(() => {
        const isDark = document.body.classList.contains('dark');
        syncThemeToggles(isDark);
    });
    observer.observe(document.body, { attributes: true, attributeFilter: ['class'] });
}

initializeTheme();
handleSystemThemeChange();
handleThemeToggleEvents();
observeThemeClass();