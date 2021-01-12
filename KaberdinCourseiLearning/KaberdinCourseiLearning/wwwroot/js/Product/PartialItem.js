var displayed = false;
function onToggleDetail(btn) {
    let Fields = $("#Fields");
    if (displayed) {
        Fields.hide(300);
        $(btn).html("<i class='fa fa-arrow-down'></i>");
    } else {
        Fields.show(300);
        $(btn).html("<i class='fa fa-arrow-up'></i>");
    }
    displayed = !displayed;
}