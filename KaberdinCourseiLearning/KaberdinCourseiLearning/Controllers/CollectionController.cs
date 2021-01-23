using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Controllers
{
    [Route("[controller]/Manage/[action]")]
    public class CollectionController : Controller
    {
        private readonly CollectionManager collectionManager;
        private readonly CustomUserManager userManager;
        private readonly ImageManager imageManager;
        private readonly ApplicationDbContext context;
        private readonly IStringLocalizer<CollectionController> localizer;

        public CollectionController(
            CollectionManager collectionManager,
            CustomUserManager userManager,
            ImageManager imageManager,
            ApplicationDbContext context,
            IStringLocalizer<CollectionController> localizer)
        {
            this.collectionManager = collectionManager;
            this.userManager = userManager;
            this.imageManager = imageManager;
            this.context = context;
            this.localizer = localizer;
        }
        public async Task<IActionResult> Create([FromBody] CreateCollectionRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, localizer["Request invalid."]));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, request.PageUserName)) return new JsonResult(new ServerResponse(false, localizer["NoCreatePermission"], "/User/Profile?name=" + request.PageUserName));
            var response = await collectionManager.CreateCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> Edit([FromBody] EditCollectionRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, localizer["Request invalid."]));
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == request.CollectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if (collection == null) return new JsonResult(new ServerResponse(false, localizer["NoCollectionError"]));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) return new JsonResult(new ServerResponse(false, localizer["NoEditPermission"], "/Collection?id=" + request.CollectionID));
            var response = await collectionManager.EditCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> UpdateImage(IFormFile file, [FromForm] int collectionID)
        {
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if (collection == null || !await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName)) return Forbid();
            await imageManager.UploadBackground(file, collectionID);
            return new OkResult();
        }
        public async Task<IActionResult> Delete(int collectionID)
        {
            var collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            var permitted = await userManager.IsUserOwnerOrAdminAsync(User, collection.User.UserName);
            if (permitted && collection != null)
            {
                context.ProductCollections.Remove(collection);
                await context.SaveChangesAsync();
                return new JsonResult(new ServerResponse(true,localizer["DefaultSuccess"]));
            }
            return new JsonResult(new ServerResponse(false,localizer["DefaultForbidden"]));
        }
    }
}
