if (localStorage.getItem('theme') === 'theme-dark') {
    setTheme('theme-dark', 'theme-light');
} else {
    setTheme('theme-light', 'theme-dark');
}
function setTheme(themeName, oldTheme) {
    document.getElementById(oldTheme).setAttribute("disabled", "true");
    localStorage.setItem('theme', themeName);
    document.getElementById(themeName).removeAttribute("disabled");
}
function toggleTheme() {
    if (localStorage.getItem('theme') === 'theme-dark') {
        setTheme('theme-light', 'theme-dark');
    } else {
        setTheme('theme-dark', 'theme-light');
    }
}

window.addEventListener("load", function (event) {
    if (localStorage.getItem('theme') === 'theme-dark') {
        setChecked(false);
    } else {
        setChecked(true);
    }
});
function setChecked(value) {
    if (value) {
        document.getElementById("themeBtn").setAttribute('checked', true);
    }
}
