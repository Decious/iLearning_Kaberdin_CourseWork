using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Hubs
{
    public class ProductHub : Hub
    {
        ProductManager productManager;
        private CustomUserManager userManager;

        public ProductHub(ProductManager productManager,CustomUserManager userManager)
        {
            this.productManager = productManager;
            this.userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            var value = Context.GetHttpContext().Request.Query["productID"];
            if (value.Count == 0) Context.Abort();
            Groups.AddToGroupAsync(Context.ConnectionId, value);
            return base.OnConnectedAsync();
        }
        [HubMethodName("sendComment")]
        public async Task SendCommentAsync(string message,string productID)
        {
            var user = await userManager.GetUserAsync(Context.User);
            if(user != null)
            {
                var roles = userManager.GetRolesAsync(user).Result.ToArray();
                var comm = new Comment() { Message = message, ProductID = int.Parse(productID), UserID = user.Id };
                await productManager.AddComment(comm);
                var response = new CommentResponse(user, comm, roles);
                await Clients.Group(productID).SendAsync("addComment", response);
            }
        }
    }
}
