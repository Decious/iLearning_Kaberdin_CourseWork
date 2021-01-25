var url, name, description, theme, columns, collectionID, dropzoneObject;
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
initHandlers();
function initHandlers() {
    $("#ItemAdd").on('click', addItemFieldInput);
    $("#SubmitBtn").on('click', prepareRequest);
    $("[name='deleteColumnBtn']").on('click', deleteColumn);
    $("[name='deleteOptionBtn']").on('click', deleteOption);
    $("[name='AddOptionBtn']").on('click', addOption);
    $("[name='ColumnType']").on('change', typeChange);
}
function addItemFieldInput() {
    let items = $("#Items");
    let clonable = $("#clonableItem");
    let clone = clonable.clone();
    removePlaceHolders(clone);
    connectEventHandlers(clone);
    items.append(clone);
    clone.show(300);
}
function removePlaceHolders(clone) {
    clone.removeAttr("id");
    clone.find("[name='PlaceholderName']").attr('name', 'ColumnName');
    clone.find("[name='PlaceholderAllowedValue']").attr('name', 'AllowedValue');
    clone.find("[name='PlaceholderType']").attr('name', 'ColumnType');
    clone.find("[name='PlaceholderDeleteBtn']").attr('name', 'deleteColumnBtn');
    clone.find("[name='PlaceholderOptionDeleteBtn']").attr('name', 'deleteOptionBtn');
    clone.find("[name='PlaceholderAddOptionBtn']").attr('name', 'AddOptionBtn');
}
function connectEventHandlers(clone) {
    clone.find("[name='ColumnType']").on('change', typeChange);
    clone.find("[name='deleteColumnBtn']").on('click', deleteColumn);
    clone.find("[name='deleteOptionBtn']").on('click', deleteOption);
    clone.find("[name='AddOptionBtn']").on('click', addOption);
}
function prepareRequest() {
    initBasicData();
    let username = $("#PageUserName");
    if (username.length != 0) sendCreateRequest(username.val());
    if (collectionID != undefined) sendEditRequest();
}
function initBasicData() {
    collectionID = $("#CollectionID").val();
    name = $("#Name").val();
    description = $("#Description").val();
    theme = $("#Theme").val();
}
function getColumns(collectionID=0) {
    let columnArr = [];
    let columnTypes = $("[name='ColumnType']");
    let columnNames = $("[name='ColumnName']");
    columnTypes.each(function (i, e) {
        let item = $(e).closest("[name='Item']");
        let columnName = columnNames.get(i);
        columnName = $(columnName).val();
        let columnID = item.attr('id');
        if (columnID == undefined) columnID = 0;
        let typeID = $(e).val();
        var allowedValues = [];
        item.find("[name='AllowedValue']").each(function (i, e) {
            allowedValues.push($(this).val());
        })
        let column = { ColumnID: +columnID, CollectionID: +collectionID, Collection: null, TypeID: +typeID, Type: null, ColumnName: columnName, AllowedValues: allowedValues.join(',') };
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
    let data = { name: name, description: description, theme: +theme, columns: columns, collectionID: +collectionID };
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
    column.hide(300, function () {
        column.remove();
    });
}
function deleteOption(event) {
    let element = $(this);
    let option = element.closest("[name='AllowedValueContainer']");
    let options = option.siblings();
    let optionsCount = options.length;
    options.each(function (i, e) {
        if ($(this).is(":animated")) optionsCount--;
    })
    if (optionsCount > 1) {
        option.hide(300, function () {
            option.remove();
            reloadNumeration(options);
        });
    } else {
        $("#NoOptionsError").show(300);
        setTimeout(function () { $("#NoOptionsError").hide(300) }, 3000);
    }
}
function reloadNumeration(options) {
    options.each(function (i, e) {
        let label = $(this).find("label");
        let html = label.html();
        let count = html.length;
        html = html.slice(0, (count-1)); //Remove number on the end of the string.
        label.html(html + (i+1));
    })
}
function addOption(event) {
    let optionsContainer = $(this).closest("[name='AllowedValues']");
    let clonable = $("#clonableOption");
    let clone = clonable.clone();
    removeOptionPlaceholders(clone);
    clone.find("[name='deleteOptionBtn']").on('click', deleteOption);
    let label = clone.find("label");
    let localizedOption = label.html();
    label.html(localizedOption + " #" + optionsContainer.children().length);
    clone.attr('name',"AllowedValueContainer");
    clone.insertBefore($(this).parent());
    clone.show(300);
}
function removeOptionPlaceholders(clone) {
    clone.removeAttr("id");
    clone.find("[name='PlaceholderAllowedValue']").attr('name', 'AllowedValue');
    clone.find("[name='PlaceholderOptionDeleteBtn']").attr('name', 'deleteOptionBtn');
}
function typeChange() {
    let options = $(this).children();
    let selectedOption = options.filter(':selected');
    let container = $(this).parent().parent().siblings("[name='AllowedValues']");
    if (selectedOption.val() == 6) {
        container.show(300);
    } else {
        container.hide(300);
    }
}