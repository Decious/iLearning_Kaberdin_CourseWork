using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.CollectionRequests;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Areas.Collection.Pages
{
    [Authorize(PolicyNames.POLICY_AUTHENTICATED)]
    public class CreateModel : PageModel
    {
        private ApplicationDbContext context;
        private CustomUserManager userManager;
        private ImageManager imageManager;
        private CollectionManager collectionManager;
        public CreateModel(CustomUserManager userManager,ApplicationDbContext context,ImageManager imageManager, CollectionManager collectionManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.imageManager = imageManager;
            this.collectionManager = collectionManager;
        }
        public CustomUser PageUser { get; set; }
        public bool isEdit { get; set; }
        public ProductCollection Collection { get; set; }
        public ProductCollectionTheme[] Themes { get; set; }
        public ColumnType[] Types { get; set; }

        public async Task<IActionResult> OnGetAsync(string name,int id)
        {
            if (name != null || id != 0)
            {
                if (await TryLoadPropertiesAsync(name,id))
                {
                    if (!await userManager.IsUserOwnerOrAdminAsync(User, PageUser.UserName)) return Forbid();
                    return Page();
                }
            }
            return Redirect("~/Index");
        }

        private async Task<bool> TryLoadPropertiesAsync(string name,int id)
        {
            if (id != 0)
            {
                Collection = await context.ProductCollections
                    .Where(c => c.CollectionID == id)
                    .Include(c => c.Columns)
                    .FirstOrDefaultAsync();
                PageUser = Collection?.User;
                isEdit = true;
            }
            else if (name != null)
            {
                PageUser = await userManager.FindByNameAsync(name);
                Collection = new ProductCollection();
                isEdit = false;
            }
            Types = context.ColumnTypes.ToArray();
            Themes = context.Themes.ToArray();
            return PageUser != null || Collection != null;
        }
        public async Task<IActionResult> OnPostCreateCollection([FromBody] CreateCollectionRequest request)
        {
            if(request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            if(!await userManager.IsUserOwnerOrAdminAsync(User,request.PageUserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to create collections for this account.","/User/Profile?name="+request.PageUserName));
            var response = await collectionManager.CreateCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> OnPostEditCollection([FromBody] EditCollectionRequest request)
        {
            if (request == null) return new JsonResult(new ServerResponse(false, "Request invalid."));
            Collection = await context.ProductCollections
                .Where(c => c.CollectionID == request.CollectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if(Collection == null) return new JsonResult(new ServerResponse(false, "Collection no longer exists."));
            if (!await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return new JsonResult(new ServerResponse(false, "You dont have permission to edit this collection.", "/Collection?id=" + request.CollectionID));
            var response = await collectionManager.EditCollectionAsync(request);
            return new JsonResult(response);
        }
        public async Task<IActionResult> OnPostUpdateImage(IFormFile file,[FromForm] int collectionID)
        {
            Collection = await context.ProductCollections
                .Where(c => c.CollectionID == collectionID)
                .Include(c => c.User)
                .FirstOrDefaultAsync();
            if (Collection == null || !await userManager.IsUserOwnerOrAdminAsync(User, Collection.User.UserName)) return Forbid();
            await imageManager.UploadBackground(file, collectionID);
            return new OkResult();
        }
    }
}
