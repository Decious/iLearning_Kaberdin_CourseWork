// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var waitIcon = $("#PleaseWait");
var alert = $("#AlertMessage");
$(document).bind("ajaxSend", function () {
    waitIcon.show();
}).bind("ajaxComplete", function () {
    waitIcon.hide();
    alert.show();
    setTimeout(resetAlertToDefaults, 3000);
}).bind("ajaxSuccess", function (event, xhr, settings) {
    let data = JSON.parse(xhr.responseText);
    if (data.status == "success")
        alert.addClass("alert-success");
    else
        alert.addClass("alert-danger");
    alert.text(data.message);
}).bind("ajaxError", function (event, jqxhr, settings, thrownError) {
    alert.addClass("alert-danger");
    alert.text("There was an error with your request...");
});
function resetAlertToDefaults() {
    alert.hide();
    if (alert.hasClass('alert-danger'))
        alert.removeClass('alert-danger');
    if (alert.hasClass('alert-success'))
        alert.removeClass('alert-success');
}
