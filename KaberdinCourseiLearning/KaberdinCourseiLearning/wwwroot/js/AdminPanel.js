var toggle = true;
var alert = $("#AlertMessage");
function roleChanged(e) {
    let changedUserId = $(e).attr("id");
    let newRole = $(e).val();
    sendRequest("Rolechange", { id: changedUserId, newRole: newRole });
}
function prepareAction(action) {
    let checkboxes = $("input[name = 'Selected']:checked");
    let ids = [];
    checkboxes.each(function (i, e) {
        ids.push($(e).val());
        updateRow($(e).closest('tr'), action);
    })
    sendRequest(action, {ids:ids});
    return false;
}
function updateRow(row,action) {
    if (action == "Delete")
        row.remove();
    if (action == "Block")
        row.children("td").last().html(locale.Banned);
    if (action == "Unblock")
        row.children("td").last().html(locale.Active);
}
function toggleClick() {
    var checkboxes = $("[name='Selected']");
    checkboxes.each(function () {
        $(this).attr("checked", toggle);
    })
    toggle = !toggle;
    return false
}


function sendRequest(action,data) {
    $.ajax({
        type: "POST",
        url: '/AdminPanel?handler=' + action,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: data
    });
}