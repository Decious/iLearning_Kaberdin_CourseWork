var displayed = false;
var likeBtn = $("#LikeBtn");
var likeCount = $("#LikeCount");
$("[name='ShowDetail']").on('click', function () {
    let Fields = $("#Fields");
    if (displayed) {
        Fields.hide(300);
        $(this).html("<i class='fa fa-arrow-down'></i>");
    } else {
        Fields.show(300);
        $(this).html("<i class='fa fa-arrow-up'></i>");
    }
    displayed = !displayed;
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
