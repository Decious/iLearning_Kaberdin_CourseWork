var displayed = false;
var likeBtn = $("#LikeBtn");
var likeCount = $("#LikeCount");
$("[name='ShowDetail']").click(function () {
    let fieldsDiv = $(this).parent().siblings("[name='FieldsDiv']");
    fieldsDiv.collapse('toggle');
});
$("[name='FieldsDiv']").each(function (i, e) {
    let btn = $(e).siblings().find("[name='ShowDetail']");
    $(e).on("hide.bs.collapse", function () {
        $(btn).html('<i class="fas fa-angle-down"></i>');
    });
    $(e).on("show.bs.collapse", function () {
        $(btn).html('<i class="fas fa-angle-up"></i>');
    });
});
likeBtn.on('click', function () {
    updateData();
    updateColor();
    updateCount();
    var id = $(this).closest("[name='Item']").attr('id');
    $.ajax({
        method: "post",
        url: "/Item/Manage/Like",
        data: { id: +id },
        global: false,
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }
    });
});
function updateData() {
    let prevData = likeBtn.data('liked');
    var liked = Boolean(prevData);
    likeBtn.data('liked', !liked);
}
function updateColor() {
    var liked = likeBtn.data('liked');
    if (liked) {
        likeBtn.css("color",'var(--danger)');
    } else {
        likeBtn.css("color",'var(--light)');
    }
}
function updateCount() {
    var liked = likeBtn.data('liked');
    var count = Number.parseInt(likeCount.html());
    if (liked) {
        count++;
    } else {
        count--;
    }
    likeCount.html(count);
}
$("#DeleteBtn").on('click', function () {
    var id = $(this).closest("[name='Item']").attr('id');
    $.ajax({
        method: "post",
        url: "/Item/Manage/Delete",
        data: { id: +id },
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data, status, jqXHR) {
            if (data.successful) {
                location.href = location.origin + data.url;
            }
        }
    });
});
