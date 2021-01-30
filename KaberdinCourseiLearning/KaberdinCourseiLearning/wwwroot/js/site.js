// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var waitIcon = $("#PleaseWait");
var alert = $("#AlertMessage");
var markdown = window.markdownit();
$(document).bind("ajaxSend", function () {
    waitIcon.show();
}).bind("ajaxComplete", function () {
    waitIcon.hide();
    alert.show();
    setTimeout(resetAlertToDefaults, 3000);
}).bind("ajaxSuccess", function (event, xhr, settings) {
    let data = JSON.parse(xhr.responseText);
    if (data.successful)
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
$("#searchbarBtn").on('click', function () {
    $("#navbar").hide(300);
    $("#header-searchbar").show(300);
})
$("#navbarBtn").on('click', function () {
    $("#header-searchbar").hide(300);
    $("#navbar").show(300);
})
$("[name='SearchBtn']").on('click', function () {
    let searchVal = $(this).closest("[name='searchbarGroup']").children("input").val();
    search(searchVal);
})
$("input[class*='searchingbar']").on('keypress', function (e) {
    if (e.which == 13) {
        let searchVal = $(this).val();
        search(searchVal);
    }
})
function search(searchVal) {
    location.href = location.origin + "/Find?q=" + encodeURIComponent(searchVal);
}
let markdownable = $("[class*='markdown']");
markdownable.each(function (i, e) {
    e = $(e);
    updateMarkdownText(e);
})
markdownable.on('change', updateMarkdownText);
function updateMarkdownText(element) {
    if (element == undefined) {
        element = $(this);
    }
    let markdownText = element.html();
    let markdownHTML = markdown.render(markdownText);
    element.html(markdownHTML);
}
function sendGenericAjaxRequest(handler) {
    $.ajax({
        method: "post",
        url: handler,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
    });
}