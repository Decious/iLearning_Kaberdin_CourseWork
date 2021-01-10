var fieldCount = 0;
function onItemAdd() {
    addItemFieldInput()
    return false;
}
function addItemFieldInput() {
    let items = $("#Items");
    let clonable = $("#clonableItem");
    let clone = clonable.clone();
    clone.removeClass("d-none");
    clone.find("input").attr('name', 'Input.ColumnNames[' + fieldCount + ']');
    clone.find("select").attr('name', 'Input.ColumnTypes[' + fieldCount + ']');
    items.append(clone);
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
            formData.append("Input.Description", $("#Description").val());
            inputs.each(function (i, e) {
                appendData(e, formData);
            });
            appendData($("#Theme"), formData);
            let types = $("select[name^='Input.ColumnTypes']");
            types.each(function (i, e) {
                appendData(e, formData);
            });
        });
        this.on("success", function (files, response) {
            location.href = location.origin + "/Collection?id=" + response;
        });
    }
}
function appendData(element, formData) {
    let e = $(element);
    let value = e.val();
    var result;
    if (isNaN(value))
        result = value;
    else
        result = +value;
    formData.append(e.attr("name"), result);
}
