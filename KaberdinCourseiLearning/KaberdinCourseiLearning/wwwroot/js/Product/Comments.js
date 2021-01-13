var productID = $("#productID").val();
const connection = new signalR.HubConnectionBuilder().withUrl("/Item/ProductHub?productID=" + productID).build();
connection.start();
connection.on("addComment", addComment);

var commentValue;
$("#commentSend").on('click', function () {
    commentValue = $("#commentInput").val();
    sendComment();
})
function sendComment() {
    connection.invoke("sendComment",commentValue);
}
function addComment(comment,user,roles) {

}