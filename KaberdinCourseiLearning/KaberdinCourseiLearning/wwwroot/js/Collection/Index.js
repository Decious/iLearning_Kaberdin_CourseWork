var tagElements = $("[name='tags']");
tagElements.each(function (i, element) {
    let e = $(element);
    let tags = e.text().split(",");
    let css = "class='badge badge-dark badge-pill'";
    var html = [];
    tags.forEach(function (v, i, arr) {
        html.push("<span "+css+">"+v+"</span>");
    })
    e.html(html);
})
$("#productsTable").on("click-cell.bs.table", function (field, value, row, $element) {
    if (value == "operations") {
        let action = $(row).data("action");
        let actionHandler = $(row).attr('href');
        doAction(actionHandler,action,$element.id);
        return false;
    }
    let href = $element._data["href"];
    if(href != undefined)
        window.location = window.origin + href;
})
$("th").addClass('hoverable');
function doAction(actionHandler,action,id) {
    if (action == "Delete") {
        $("#productsTable").bootstrapTable('remove', { field: 'id', values: [id] })
    }
    sendGenericAjaxRequest(actionHandler);
}
