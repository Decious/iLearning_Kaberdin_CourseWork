var displayed = false;
function onToggleDetail(btn) {
    let Fields = $("#Fields");
    if (displayed) {
        Fields.hide(300);
        $(btn).html("<i class='fa fa-arrow-down'></i>");
    } else {
        Fields.show(300);
        $(btn).html("<i class='fa fa-arrow-up'></i>");
    }
    displayed = !displayed;
}
$("#DeleteBtn").on('click', function () {
    var id = $(this).closest("[name='Item']").attr('id');
    $.ajax({
        method: "post",
        url: "/Item?handler=DeleteProduct",
        data: {id:+id},
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
        success: function (data, status, jqXHR) {
            if (data.successful) {
                location.href = location.origin + data.url;
            }
        }
    });
})
