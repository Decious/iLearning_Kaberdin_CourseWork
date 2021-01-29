$("[name='colDelete']").on('click', Delete);
$("[name='colEdit']").on('click', Edit);
$("[name='CollapsableDescriptionDiv']").each(function (i, e) {
    let btn = $(e).parent().find("[name='ShowDetail']");
    $(e).on("hide.bs.collapse", function () {
        $(btn).html('<i class="fas fa-angle-down"></i>');
    });
    $(e).on("show.bs.collapse", function () {
        $(btn).html('<i class="fas fa-angle-up"></i>');
    });
});
$("[name='ShowDetail']").each(function (i, e) {
    $(e).click(toggleDetail);
})
$("[class*='ViewableCollection']").each(function (i, e) {
    $(e).click(viewCollection);
})
function viewCollection(event) {
    if ($(event.target).is("div")) {
        let collectionID = $(event.target).closest("[name='Collection']").attr("id");
        location.href = location.origin + "/Collection?id=" + collectionID;
    }
}
function Delete() {
    let collectionID = $(this).closest("[name='Collection']").attr("id");
    sendRequest("Delete", { collectionID: collectionID })
}
function Edit() {
    let collectionID = $(this).closest("[name='Collection']").attr("id");
    location.href = location.origin + "/Collection/Create?id=" + collectionID;
}
function toggleDetail(event) {
    let collection = $(event.target).closest("[name='Collection']");
    let description = collection.find("[name='CollapsableDescriptionDiv']");
    description.collapse("toggle");
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