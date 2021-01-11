var values = $("[name='columnValue']");
values.each(function (i,e) {
    let value = $(e).text();
    if (value == "on") $(e).text('Yes');
    if (value == "off") $(e).text('No');
})
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