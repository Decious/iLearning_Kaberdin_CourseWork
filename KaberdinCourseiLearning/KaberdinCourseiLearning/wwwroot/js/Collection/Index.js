var names = ['ID','Name','Tags'];
$("[name='ColumnName']").each(function (i, e) {
    names.push($(e).data('sort'));
});
var options = {
    valueNames: names
};
var productList = new List('products', options);
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
$(".product").click(function () {
    window.location = window.origin+$(this).attr("href");
})