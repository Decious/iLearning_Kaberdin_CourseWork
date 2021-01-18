var tagsinput = $("#Tags")
var whitelist = tagsinput.attr('whitelist').split(",");
tagsinput.tagify({
    enforceWhitelist: false,
    delimiters: null,
    enabled: 1,
    maxTags: 3,
    maxItems: 5,
    originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(','),
    whitelist: whitelist
});

var values = [];
var columnIds = [];
var tags;
var columnValues = [];
var name;
var productID;
var collectionID;
$("#submitBtn").on('click', function () {
    name = $("#Name").val();
    tags = tagsinput.val();
    getIDs();
    getColumnValues();
    if (productID != 0) sendEditRequest();
    if (collectionID != 0) sendCreateRequest();
    return false;
})
function getIDs() {
    if ($("#productID").length > 0) {
        productID = $("#productID").val();
    } else productID = 0;
    if ($("#collectionID").length > 0) {
        collectionID = $("#collectionID").val();
    } else collectionID = 0;
}

function getColumnValues() {
    let valueInputs = $("[name='columnValue']");
    valueInputs.each(function (i, e) {
        let value = "";
        if ($(e).attr('type') == "checkbox") {
            value = ""+$(e).is(":checked");
        } else {
            value=$(e).val();
        }
        let columnId = $(e).attr('id');
        columnIds.push(+columnId);
        var columnValue = { ProductID: +productID, Product: null, ColumnID: +columnId, Column: null, Value: value, SearchVector: null };
        columnValues.push(columnValue);
    })
}

function sendCreateRequest() {
    let data = { name: name, tags: tags, columnValues: columnValues, collectionID: +collectionID };
    sendRequest(data, "CreateProduct");
}
function sendEditRequest() {
    let data = { name: name, tags: tags, columnValues: columnValues, productID: +productID };
    sendRequest(data, "EditProduct");
}
function sendRequest(data, handler) {
    let json = JSON.stringify(data);
    $.ajax({
        method: "post",
        contentType: "application/json",
        url: "/Item/Create?handler=" + handler,
        data: json,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data, status, jqXHR) {
            if (data.successful) {
                location.href = location.origin + data.url;
            }
        }
    });
}
