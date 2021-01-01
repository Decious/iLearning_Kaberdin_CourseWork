var toggle = true;
var alert = $("#AlertMessage");
var queryString = window.location.search;
var urlParams = new URLSearchParams(queryString);
checkParams();
function checkParams() {
    let resultIsSuccess = urlParams.get('resultIsSuccess');
    if (resultIsSuccess != undefined) {
        if (resultIsSuccess) success(); else failed();
    }
}
function roleChanged(e) {
    prepareAction("Role");
    let changedUserId = $(e).attr("id");
    prepareSelection(changedUserId)
    let newRole = $(e).val();
    prepareRole(newRole);
    $("form").submit();
}
function prepareAction(action) {
    let hiddenFormAction = $("#FormActionInput");
    hiddenFormAction.val(action);
    startWaiting();
}
function prepareSelection(changedUserId) {
    let checkboxes = $("[name='Selected']");
    checkboxes.each(function () {
        if ($(this).val() == changedUserId) {
            $(this).attr("checked", true);
        } else {
            $(this).attr("checked", false);
        }
    })
}
function prepareRole(newRole) {
    let hiddenNewRole = $("#NewRole");
    hiddenNewRole.val(newRole);
}
function startWaiting() {
    let waitIcon = $("#PleaseWait");
    waitIcon.removeClass("d-none");
    waitIcon.addClass("d-flex");
}
function toggleClick() {
    var checkboxes = $("[name='Selected']");
    checkboxes.each(function () {
        $(this).attr("checked", toggle);
    })
    toggle = !toggle;
    return false
}
function showAlert() {
    alert.addClass("alert");
    alert.show();
}
function success() {
    alert.addClass("alert-success");
    alert.text("Action completed successfully.");
    showAlert();
}
function failed() {
    alert.addClass("alert-danger");
    let errorMessage = urlParams.get('errorMessage');
    alert.text("Action failed to complete." + errorMessage);
    showAlert();
}