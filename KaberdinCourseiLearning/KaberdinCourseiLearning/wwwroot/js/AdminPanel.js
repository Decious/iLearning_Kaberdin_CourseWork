var names = ['Name', 'Email', 'Role', 'Status'];
var checkboxes = $("input[name = 'Selected']:checked");
var options = {
    valueNames: names
};
var usersList = new List('users', options);

var toggle = true;
var alert = $("#AlertMessage");
function roleChanged(e) {
    let changedUserId = $(e).attr("id");
    let newRole = $(e).val();
    sendRequest("Rolechange", { id: changedUserId, newRole: newRole });
}
function prepareAction(action) {
    checkboxes = $("input[name = 'Selected']:checked");
    let ids = [];
    checkboxes.each(function (i, e) {
        ids.push($(e).val());
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
function revertRoleChange(select) {
    let roles = select.children();
    roles.each(function (i, e) {
        if ($(e).html() != select.val())
            select.val($(e).html());
    })
}
function toggleClick() {
    checkboxes = $("[name='Selected']");
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
        data: data,
        success: function (response) {
            if (response.successful) {
                checkboxes.each(function (i, e) {
                    updateRow($(e).closest('tr'), action);
                })
            } else {
                if (action == "Rolechange") {
                    revertRoleChange($("#" + data.id));
                }
            }
        }
    });
}