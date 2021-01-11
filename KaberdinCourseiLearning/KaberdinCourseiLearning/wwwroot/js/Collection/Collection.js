let descriptions = $("[name='Description']");
let markdown = window.markdownit();
descriptions.each(function (i, e) {
    e = $(e);
    let markdownText = e.html();
    let markdownHTML = markdown.render(markdownText);
    e.html(markdownHTML);
});
function ViewCollection(div) {
    let collectionID = $(div).closest("[name='Collection']").attr("id");
    location.href = location.origin + "/Collection?id=" + collectionID;
}
function onDelete(btn) {
    let collectionID = $(btn).closest("[name='Collection']").attr("id");
    sendRequest("DeleteCollection", { collectionID: collectionID })
}
function onToggleDetail(btn) {
    let collection = $(btn).closest("[name='Collection']");
    if (collection.hasClass('expand-collection')) {
        collection.removeClass('expand-collection');
        $(btn).html("<i class='fa fa-arrow-down'></i>");
    } else {
        collection.addClass('expand-collection');
        $(btn).html("<i class='fa fa-arrow-up'></i>");
    }
}
function sendRequest(handler, data) {
    $.ajax({
        type: "POST",
        url: '/Collection?handler='+handler,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: data
    }).done(function () {
        location.reload();
    })
}