var description = $("#DescriptionContent");
var invoker = $("#ProfileDescription");
init();

function init() {
    prepareDropzone();
}
function setDescAsTextArea() {
    let text = description.children("p").text();
    description.html("<textarea class='text-primary' style='width: 100%; height: 100%; background: transparent; border:none; resize:none;' >" + text + "</textarea>");
    var textarea = description.children("textarea");
    textarea.focus();
    $('#DescriptionContent').off('click');
    textarea.on('keypress', function (e) {
        if (e.which == 13) {
    acceptDescription();
        }
    });
}
function acceptDescription() {
    let newText = description.children("textarea").val();
    sendDescToServer(newText);
    setDescText(newText);
    $('#DescriptionContent').on('click', function () {
        setDescAsTextArea();
    });
}
function sendDescToServer(newText) {
    $.ajax({
        type: "POST",
        url: '/User/Profile?handler=AcceptDescription',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        data: { newText: newText, name: $("#UserName").text()}
    });
}
function setDescText(newText) {
    description.html("<p class='text-break text-wrap text-truncate' >" + newText + "</p>");
}
function prepareDropzone() {
    Dropzone.options.AvatarDropzone = {
        maxFilesize: 4,
        maxFiles: 1,
        acceptedFiles: "image/*",
        resizeWidth: 205,
        resizeHeight: 205,
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
            $("[class='dz-button']").hide().css({'background-color':'rgba(255,255,255,.7)'});
            this.on("success", function(file,response) {
                $("#AvatarPicture").attr('src', response.url);
                $("#AvatarDropzone").css('background-image', "url("+response.url+")");
                this.removeAllFiles(true);
            });
            this.on("sending", function (file, xhr, formData) {
                formData.append("name", $("#UserName").text());
            });
        }
    }
}
$('#AvatarDropzone').on('dragenter', function () {
    $(this).addClass('border', 'border-primary');
        $("[class='dz-button']").show();
});

$('#AvatarDropzone').on('dragleave', function () {
    $(this)
        .removeClass('border', 'border-primary');
        $("[class='dz-button']").hide();
});
$('#AvatarDropzone').on('mouseenter', function () {
    $(this)
        .addClass('border', 'border-primary');
        $("[class='dz-button']").show();
});

$('#AvatarDropzone').on('mouseleave', function () {
    $(this)
        .removeClass('border', 'border-primary');
        $("[class='dz-button']").hide();
});

$('#DescriptionContent').on('mouseenter', function () {
    $(this)
        .addClass('border', 'border-primary');
});

$('#DescriptionContent').on('mouseleave', function () {
    $(this)
        .removeClass('border', 'border-primary');
});
$('#DescriptionContent').on('click', function () {
    setDescAsTextArea();
});