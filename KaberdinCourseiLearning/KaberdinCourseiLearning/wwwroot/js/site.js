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
}).bind("ajaxSuccess", function (event, xhr, settings) {
    resetAlertToDefaults();
    let data = JSON.parse(xhr.responseText);
    $("#FormErrors").html('');
    if (data.successful)
        alert.addClass("alert-success");
    else 
        alert.addClass("alert-danger");
    var messages = data.message;
    displayMessages(messages);
}).bind("ajaxError", function (event, jqxhr, settings, thrownError) {
    alert.addClass("alert-danger");
    alert.text("There was an error with your request...");
});
function displayMessages(messages) {
    if (messages.length == 1) {
        alert.text(messages[0]);
        showAlert();
    }
    else {
        messages.forEach(function (v, i, a) {
            $("#FormErrors").prepend("<p class='text-danger'><strong>" + v + "</strong><p>");
        })
    }
}
function showAlert() {
    alert.show();
    setTimeout(resetAlertToDefaults, 5000);
}
function resetAlertToDefaults() {
    alert.hide();
    if (alert.hasClass('alert-danger'))
        alert.removeClass('alert-danger');
    if (alert.hasClass('alert-success'))
        alert.removeClass('alert-success');
}
function search(searchVal) {
    location.href = location.origin + "/Find?q=" + encodeURIComponent(searchVal);
}
let markdownable = $("[class*='markdown']");
markdownable.each(function (i, e) {
    e = $(e);
    updateMarkdownText(e);
})
function updateMarkdownText(element) {
    if (element == undefined) {
        element = $(this);
    }
    let markdownText = element.html();
    let markdownHTML = markdown.render(markdownText);
    element.html(markdownHTML);
}

markdownable.on('change', updateMarkdownText);
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


function sendGenericAjaxRequest(handler) {
    $.ajax({
        method: "post",
        url: handler,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
    });
}