let descriptions = $("[name='Description']");
let markdown = window.markdownit();
descriptions.each(function (i, e) {
    e = $(e);
    let markdownText = e.html();
    let markdownHTML = markdown.render(markdownText);
    e.html(markdownHTML);
});

function onDelete(btn) {
    let collectionID = $(btn).closest("[name='Collection']").attr("id");
    sendRequest("DeleteCollection", { collectionID: collectionID, name: $("#UserName").text() })
}
function sendRequest(handler, data) {
    $.ajax({
        type: "POST",
        url: '/User/Profile?handler='+handler,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: data
    }).done(function () {
        location.reload();
    })
}