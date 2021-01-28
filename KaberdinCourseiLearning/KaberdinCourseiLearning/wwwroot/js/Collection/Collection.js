$("[name='colDelete']").on('click', Delete);
$("[name='colEdit']").on('click', Edit);
function ViewCollection(div) {
    let collectionID = $(div).closest("[name='Collection']").attr("id");
    location.href = location.origin + "/Collection?id=" + collectionID;
}
function Delete() {
    let collectionID = $(this).closest("[name='Collection']").attr("id");
    sendRequest("Delete", { collectionID: collectionID })
}
function Edit() {
    let collectionID = $(this).closest("[name='Collection']").attr("id");
    location.href = location.origin + "/Collection/Create?id=" + collectionID;
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
        url: '/Collection/Manage/'+handler,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: data
    }).done(function () {
        location.reload();
    })
}