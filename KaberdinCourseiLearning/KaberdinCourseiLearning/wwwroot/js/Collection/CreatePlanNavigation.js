var currentItem = 0;
var planItemHeaders = ["PlanCollection", "PlanItems", "PlanPublish"];
var PlanItems = $("[name='PlanItem']");

function onNext() {
    let futureItem = currentItem + 1;
    updatePage(futureItem)
    return false;
}
function onPrev() {
    let futureItem = currentItem - 1;
    updatePage(futureItem)
    return false;
}
function updatePage(futureItem) {
    $(PlanItems[currentItem]).addClass("d-none");
    currentItem = futureItem;
    $(PlanItems[currentItem]).removeClass("d-none");
    updateButtons();
    updatePlan();
}
function updateButtons() {
    let NextBtn = $("#Next");
    let PrevBtn = $("#Previous");
    let SubmitBtn = $("#SubmitBtn");
    if (currentItem == 2) {
        NextBtn.addClass("d-none");
        SubmitBtn.removeClass("d-none");
        updatePublishInfo();
    } else {
        NextBtn.removeClass("d-none");
        SubmitBtn.addClass("d-none");
    }
    let canPrev = currentItem > 0;
    if (canPrev) {
        PrevBtn.removeClass("disabled");
    } else {
        PrevBtn.addClass("disabled");
    }
    PrevBtn.attr("disabled", !canPrev);
}
function updatePublishInfo() {

}
function updatePlan() {
    $("#" + planItemHeaders[currentItem - 1]).removeClass("btn-primary");
    $("#" + planItemHeaders[currentItem + 1]).removeClass("btn-primary");
    $("#" + planItemHeaders[currentItem]).addClass("btn-primary");
}