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
function onSubmit() {
    let name = $("#Name").val();
    let tags = tagsinput.val();
    getColumnValues();
    let createRequest = { name: name, tags: tags, columnValues: values, columnIDs: columnIds };
    sendRequest(createRequest);
    return false;
}
function getColumnValues() {
    let valueInputs = $("[name='columnValue']");
    values = [];
    columnIds = [];
    valueInputs.each(function (i, e) {
        if ($(e).attr('type') == "checkbox") {
            values.push(""+$(e).is(":checked"));
        } else {
            values.push($(e).val());
        }
        let columnId = $(e).attr('id');
        columnIds.push(+columnId);
    })
}
function sendRequest(data) {
    let json = JSON.stringify(data);
    $.ajax({
        method: "post",
        contentType: "application/json",
        data: json,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data, status, jqXHR) {
            if (data.successful) {
                window.setTimeout(function () { location.href = location.origin + data.url; },1000)
            }
        }
    });
}
