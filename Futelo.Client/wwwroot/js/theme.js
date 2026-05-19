window.themeInterop = {
    get: () => localStorage.getItem('futelo_theme'),
    set: (theme) => {
        localStorage.setItem('futelo_theme', theme);
        themeInterop.apply(theme);
    },
    apply: (theme) => {
        if (theme === 'light') {
            document.body.classList.add('light');
        } else {
            document.body.classList.remove('light');
        }
    }
};
