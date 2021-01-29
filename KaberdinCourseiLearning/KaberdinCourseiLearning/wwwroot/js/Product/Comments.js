var productID = $("#productID").val();
var clonableComment = $("#ClonableComment");
const connection = new signalR.HubConnectionBuilder().withUrl("/Item/ProductHub?productID=" + productID).build();
connection.start();
connection.on("addComment", addComment);

var commentValue;
$("#commentSend").on('click', function () {
    sendCommentInputHandler();
})
$("#commentInput").on('keypress', function (e) {
    if (e.which == 13)
        sendCommentInputHandler();
})
function sendCommentInputHandler() {
    commentValue = $("#commentInput").val();
    sendComment();
    $("#commentInput").val('');
}
function sendComment() {
    connection.invoke("sendComment", commentValue, productID);
}
function addComment(response) {
    var clone = clonableComment.clone();
    clone.find(".Avatar").attr('src', response.avatarUrl);
    var roles = "";
    response.roles.forEach(function (val, i, arr) {
        roles += "<span class='badge badge-dark'>" + val + "</span>";
    })
    clone.find(".UserName").html(response.userName + " " + roles);
    clone.find(".Date").html(response.creationDate);
    clone.find(".comment-body").html(response.message);
    clone.attr('id', response.commentID);
    $("[class*='CommentBoxContent']").prepend(clone);
    clone.show(300);
}