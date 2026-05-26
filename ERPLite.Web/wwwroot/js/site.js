// ============================================
// ERPLite - Main JS File
// File: wwwroot/js/site.js
// ============================================

// Active nav link highlight based on current URL
document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-item-custom a').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && href !== '#' && currentPath.startsWith(href)) {
            link.classList.add('active');
        }
    });
});
