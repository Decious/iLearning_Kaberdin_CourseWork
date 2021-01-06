let descriptions = $("[name='Description']");
let markdown = window.markdownit();
descriptions.each(function (i, e) {
    e = $(e);
    let markdownText = e.html();
    let markdownHTML = markdown.render(markdownText);
    e.html(markdownHTML);
});
if (isPermitted()) {
    var buttonEdit = $("#DescChange");
    var buttonAccept = $("#DescAccept");
    var description = $("#DescriptionContent");
    var invoker = $("#ProfileDescription");
    init();
}
function isPermitted() {
    return ($("#AvatarChange").length > 0);
}
function init() {
    prepareAvatar();
    prepareDescription();
    prepareDropzone();
}
function prepareAvatar() {
    prepareMouseEvents($("#ProfilePicture"), $("#AvatarChange"));
}
function prepareMouseEvents(invokerElement, subject) {
    invokerElement.mouseenter(function () {
        subject.show(0.5);
    })
    invokerElement.mouseleave(function () {
    subject.hide(0.5);
    })
}
function prepareDescription() {
    prepareMouseEvents(invoker, buttonEdit);
    prepareDescOnEdit();
    prepareEditAccept();
}
function prepareDescOnEdit() {
    buttonEdit.on('click', function () {
        setDescAsTextArea();
        unBindMouseEvents(invoker);
        buttonAccept.show();
        buttonEdit.hide();
    })
}
function setDescAsTextArea() {
    let text = description.children("p").text();
    description.html("<textarea style='width: 100%; height: 100%; background: transparent;border: none;' >" + text + "</textarea>");
    var textarea = description.children("textarea");
    textarea.focus();
    textarea.on('keypress', function (e) {
        if (e.which == 13) {
    acceptDescription();
        }
    });
}
function unBindMouseEvents(element) {
    element.unbind("mouseenter");
    element.unbind("mouseleave");
}

function prepareEditAccept() {
    buttonAccept.on('click', function () {
        acceptDescription();
    })
}
function acceptDescription() {
    let newText = description.children("textarea").val();
    sendDescToServer(newText);
    setDescText(newText);
    prepareMouseEvents(invoker, buttonEdit);
    buttonAccept.hide();
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
        init: function () {
            this.on("success", function (event) { setTimeout(location.reload(), 2000); });
            this.on("sending", function (file, xhr, formData) {
                formData.append("name", $("#UserName").text());
            });
        }
    }
}
