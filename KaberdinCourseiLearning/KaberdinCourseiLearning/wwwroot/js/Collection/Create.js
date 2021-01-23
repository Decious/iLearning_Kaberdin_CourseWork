var url;
var name;
var description;
var theme;
var columns;
var collectionID;
var deletedColumns = [];
var dropzoneObject;
Dropzone.autoDiscover = false;
$("#backgroundImageDz").dropzone({
    autoProcessQueue: false,
    method: "post",
    ParallelUploads: 100,
    maxFiles: 1,
    maxFilesize: 8,
    url: location.origin + "/Collection/Manage/UpdateImage",
    headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
    paramName: "file",
    acceptedFiles: 'image/*',
    dictDefaultMessage: dropzoneLocale.dictDefaultMessage,
    dictCancelUpload: dropzoneLocale.dictCancelUpload,
    dictCancelUploadConfirmation: dropzoneLocale.dictCancelUploadConfirmation,
    dictFallbackMessage: dropzoneLocale.dictFallbackMessage,
    dictFallbackText: dropzoneLocale.dictFallbackText,
    dictFileTooBig: dropzoneLocale.dictFileTooBig,
    dictInvalidFileType: dropzoneLocale.dictInvalidFileType,
    dictMaxFilesExceeded: dropzoneLocale.dictMaxFilesExceeded,
    dictRemoveFile: dropzoneLocale.dictRemoveFile,
    dictResponseError: dropzoneLocale.dictResponseError,
    init: function () {
        dropzoneObject = this;
    },
    success: function (file, response) {
        location.href = location.origin + url;
    },
    sending: function (file, xml, formData) {
        formData.append("collectionID", collectionID)
    }
})
$("#ItemAdd").on('click', addItemFieldInput);
$("#SubmitBtn").on('click', prepareRequest);
$("[name='deleteColumnBtn']").on('click', deleteColumn);
function addItemFieldInput() {
    let items = $("#Items");
    let clonable = $("#clonableItem");
    let clone = clonable.clone();
    clone.removeClass("d-none");
    clone.removeAttr("id");
    clone.find("[name='PlaceholderName']").attr('name', 'ColumnName');
    clone.find("[name='PlaceholderType']").attr('name', 'ColumnType');
    clone.find("[name='PlaceholderDeleteBtn']").attr('name', 'deleteColumnBtn').on('click',deleteColumn);
    items.append(clone);
}
function prepareRequest() {
    let username = $("#PageUserName");
    collectionID = $("#CollectionID").val();
    name = $("#Name").val();
    description = $("#Description").val();
    theme = $("#Theme").val();
    if (username.length != 0) sendCreateRequest(username.val());
    if (collectionID != undefined) sendEditRequest();
}
function getColumns(collectionID=0) {
    let columnArr = [];
    let columnTypes = $("[name='ColumnType']");
    let columnNames = $("[name='ColumnName']");
    columnTypes.each(function (i, e) {
        let columnName = columnNames.get(i);
        columnName = $(columnName).val();
        let columnID = $(e).closest("[name='Item']").attr('id');
        if (columnID == undefined) columnID = 0;
        let typeID = $(e).val();
        let column = { ColumnID: +columnID, CollectionID: +collectionID, Collection: null, TypeID: +typeID, Type:null, ColumnName: columnName };
        columnArr.push(column);
    })
    return columnArr;
}
function sendCreateRequest(username) {
    columns = getColumns();
    let data = { name: name, description: description, theme: +theme, columns: columns, pageUserName: username };
    sendRequest(data, "Create");
}
function sendEditRequest() {
    columns = getColumns(collectionID);
    let data = { name: name, description: description, theme: +theme, columns: columns, collectionID: +collectionID, deletedColumns: deletedColumns };
    sendRequest(data, "Edit");
}
function sendRequest(data,handler) {
    let json = JSON.stringify(data);
    $.ajax({
        method: "post",
        contentType: "application/json",
        url: "/Collection/Manage/"+handler,
        data: json,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data, status, jqXHR) {
            if (data.successful) {
                url = data.url;
                let regex = /[?|&](?:id|collectionID)=(\w*)/;
                collectionID = regex.exec(url)[1];
                if (dropzoneObject.files.length == 0) location.href = location.origin + url;
                dropzoneObject.processQueue();
            }
        }
    });
}
function deleteColumn(event) {
    let element = $(this);
    let column = element.closest("[name='Item']");
    let id = column.attr('id');
    if (id != undefined) deletedColumns.push(id);
    column.remove();
}

