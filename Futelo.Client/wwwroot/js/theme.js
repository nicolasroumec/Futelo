window.themeInterop = {
    key: 'futelo_theme',

    apply: function (theme) {
        if (theme === 'light') {
            document.body.classList.add('light');
        } else {
            document.body.classList.remove('light');
        }
    },

    get: function () {
        return localStorage.getItem(this.key);
    },

    set: function (theme) {
        localStorage.setItem(this.key, theme);
        this.apply(theme);
    }
};
