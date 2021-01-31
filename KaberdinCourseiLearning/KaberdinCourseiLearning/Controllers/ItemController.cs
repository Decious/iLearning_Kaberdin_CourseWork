using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Data.ProductRequests;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Controllers
{
    [Route("[controller]/Manage/[action]")]
    public class ItemController : Controller
    {
        private ApplicationDbContext context;
        private CustomUserManager userManager;
        private ProductManager productManager;
        private IStringLocalizer<ItemController> localizer;
        private bool PermittedToChange;

        public ItemController(
            ApplicationDbContext context,
            CustomUserManager userManager,
            ProductManager productManager,
            IStringLocalizer<ItemController> localizer)
        {
            this.context = context;
            this.userManager = userManager;
            this.productManager = productManager;
            this.localizer = localizer;
        }
        public async Task<JsonResult> Delete(int id)
        {
            var product = await context.Products
                .Where(p => p.ProductID == id)
                .Include(p => p.Collection)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
            if (product == null) return new JsonResult(new ServerResponse(false, localizer["Product doesnt exist"], "/"));
            PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, product.Collection.User.UserName);
            if (PermittedToChange)
            {
                var coll_ID = product.CollectionID;
                await productManager.DeleteProductAsync(id);
                return new JsonResult(new ServerResponse(true, localizer["Product successfully deleted."], "/Collection?id=" + coll_ID));
            }
            return new JsonResult(new ServerResponse(false, localizer["NoPermissionCreateError"]));
        }
        public async Task<JsonResult> Like(int id)
        {
            var product = await context.Products.FindAsync(id);
            var guestUser = await userManager.GetUserAsync(User);
            if (product != null && guestUser != null)
            {
                var like = await context.Likes.Where(l => l.ProductID == id && l.UserID == guestUser.Id).FirstOrDefaultAsync();
                if (like == null)
                    context.Likes.Add(new Like() { ProductID = id, UserID = guestUser.Id });
                else
                    context.Likes.Remove(like);
                await context.SaveChangesAsync();
                return new JsonResult(new ServerResponse(true, localizer["Sucessful"]));
            }
            return new JsonResult(new ServerResponse(false, localizer["NotAuthorizedError"]));
        }
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new ServerResponse(false, ModelState.GetModelErrors()));
            if (request == null) return new JsonResult(new ServerResponse(false, localizer["Request invalid."]));
            var collection = await context.ProductCollections.Where(c => c.CollectionID == request.CollectionID).Include(c => c.User).FirstOrDefaultAsync();
            if (collection == null) return new JsonResult(new ServerResponse(false, localizer["Collection no longer exists."]));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) return new JsonResult(new ServerResponse(false, localizer["NoPermissionCreateError"], "/Collection?collectionID=" + request.CollectionID));

            var response = await productManager.CreateProductAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> Edit([FromBody] ProductEditRequest request)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new ServerResponse(false, ModelState.GetModelErrors()));
            if (request == null) return new JsonResult(new ServerResponse(false, localizer["Request invalid."]));
            var product = await context.Products.FindAsync(request.ProductID);
            if (product == null) return new JsonResult(new ServerResponse(false, localizer["Item no longer exists."]));
            var collection = await context.ProductCollections.Where(c => c.CollectionID == product.CollectionID).Include(c => c.User).FirstOrDefaultAsync();
            if (collection == null) return new JsonResult(new ServerResponse(false, localizer["Collection no longer exists."]));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) return new JsonResult(new ServerResponse(false, localizer["NoPermissionEditError"], "/Item?id=" + request.ProductID));

            var response = await productManager.EditProductAsync(request);
            return new JsonResult(response);
        }
    }
}
