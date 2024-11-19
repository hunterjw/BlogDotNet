document.addEventListener('DOMContentLoaded', (e) => {

    // update on initial page load
    updateTheme();

    // then update on partial page reloads
    Blazor.addEventListener('enhancedload', () => {
        updateTheme();
    });
})

function updateTheme() {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        // dark mode
        document.documentElement.setAttribute('data-bs-theme', 'dark');
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'light');
    }
}