var fieldCount = 0;
function onItemAdd() {
    addItemFieldInput()
    return false;
}
function addItemFieldInput() {
    let items = $("#Items");
    items.append(
    "<div class=\"form-group\">" +
        "<div class=\"container\">" +
            "<div class=\"row\">" +
                "<div class=\"col-6\">" +
                    "<input name=\"Input.ColumnNames["+fieldCount+"]\" class=\"form-control\"/>" +
                "</div>" +
                "<div class=\"col-6\">" +
                    "<select name=\"Input.ColumnTypes["+fieldCount+"]\" class=\"form-control\">" +
                    "<option>String</option>" +
                    "<option>TextArea</option>" +
                    "<option>Number</option>" +
                    "<option>Boolean</option>" +
                    "<option>Date</option>" +
                    "</select>" +
                "</div>" +
            "</div>" +
        "</div>" +
    "</div>");
    fieldCount++;
}
Dropzone.options.backgroundImageDz = {
    autoProcessQueue: false,
    ParallelUploads: 100,
    maxFiles: 1,
    maxFilesize: 8,
    url: location.href,
    headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
    paramName:"file",
    acceptedFiles:'image/*',
    init: function () {
        dropzone = this;
        $("button[type='submit']").on('click', function (e) {
            if (dropzone.getQueuedFiles().length > 0) {
                e.preventDefault();
                e.stopPropagation();
                dropzone.processQueue();
            } 
        });
        this.on("sending", function (file, xhr, formData) {
            let inputs = $("input");
            let selects = $("select");
            formData.append("Input.Description", $("#Description").val());
            inputs.each(function (i, e) {
                appendData(e, formData);
            });
            selects.each(function (i, e) {
                appendData(e, formData);
            });
        });
        this.on("success", function (files, response) {
            location.href = location.origin + "/Collection?id=" + response;
        });
    }
}
function appendData(element, formData) {
    e = $(element);
    formData.append(e.attr("name"), e.val());
}