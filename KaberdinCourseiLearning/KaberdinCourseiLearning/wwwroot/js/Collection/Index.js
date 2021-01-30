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
    if (value == "operations") return
    let href = $element._data["href"];
    if(href != undefined)
        window.location = window.origin + href;
})
$("th").addClass('hoverable');

function actionHandler(link) {
    let actionHandler = $(link).attr('href');
    let id = getParameterByName("id", actionHandler);
    $("#productsTable").bootstrapTable('remove', { field: 'id', values: [id] });
    sendGenericAjaxRequest(actionHandler);
    return false;
}

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}