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
function toggleTheme(inputElement) {
    if (inputElement.checked)
        setChecked();
    else
        removeChecked();
    if (localStorage.getItem('theme') === 'theme-dark') {
        setTheme('theme-light', 'theme-dark');
    } else {
        setTheme('theme-dark', 'theme-light');
    }
}

window.addEventListener("load", function (event) {
    if (localStorage.getItem('theme') === 'theme-dark') {
        removeChecked();
    } else {
        setChecked();
    }
});
function setChecked() {
    document.getElementsByName("themeBtn").forEach(function (v, i, a) {
        v.checked = true;
    })
}
function removeChecked() {
    document.getElementsByName("themeBtn").forEach(function (v, i, a) {
        v.checked = false;
    })
}
