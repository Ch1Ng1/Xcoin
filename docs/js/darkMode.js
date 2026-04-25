// Dark mode management
window.darkMode = {
    key: 'xcoin-dark-mode',

    isDark: function () {
        return document.documentElement.classList.contains('dark-mode');
    },

    toggle: function () {
        const isDark = document.documentElement.classList.toggle('dark-mode');
        localStorage.setItem(window.darkMode.key, isDark ? '1' : '0');
        return isDark;
    },

    set: function (dark) {
        if (dark) {
            document.documentElement.classList.add('dark-mode');
        } else {
            document.documentElement.classList.remove('dark-mode');
        }
        localStorage.setItem(window.darkMode.key, dark ? '1' : '0');
    },

    init: function () {
        const saved = localStorage.getItem(window.darkMode.key);
        if (saved === '1') {
            document.documentElement.classList.add('dark-mode');
        }
    }
};

// Apply immediately to avoid flash
window.darkMode.init();
