using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace KaberdinCourseiLearning.Areas.User.Pages
{
    public class ProfileModel : PageModel
    {
        private CustomUserManager userManager;
        private ApplicationDbContext context;
        private ImageManager imageManager;
        private IStringLocalizer<ProfileModel> localizer;

        public bool PermittedToChange { get; set; }
        public CustomUser PageUser { get; set; }
        public ProfileModel(CustomUserManager userManager,
            ApplicationDbContext context,
            ImageManager imageManager,
            IStringLocalizer<ProfileModel> localizer)
        {
            this.userManager = userManager;
            this.context = context;
            this.imageManager = imageManager;
            this.localizer = localizer;
        }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            if (name != null)
            {
                var loaded = await TryLoadPropertiesAsync(name);
                if (loaded)
                {
                    return Page();
                }
            }
            return Redirect("~/Index");
        }
        private async Task<bool> TryLoadPropertiesAsync(string pageUserName)
        {
            PageUser = await context.Users
                .Where(u => u.UserName == pageUserName)
                .Include(u => u.HomePage)
                .Include(u => u.ItemCollections)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            bool result = PageUser != null;
            if (result)
            {
                PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, pageUserName);
            }
            return result;
        }
        public async Task<IActionResult> OnPostUploadAvatar(IFormFile file, string name)
        {
            if (await isLoadedAndPermittedToChange(name))
            {
                _ = await imageManager.UploadAvatar(file, PageUser.Id);
                return new JsonResult(new ServerResponse(true, localizer["Avatar was uploaded successfully"]));
            }
            return new JsonResult(new ServerResponse(false, localizer["AvatarPermError"]));
        }
        private async Task<bool> isLoadedAndPermittedToChange(string pageUserName)
        {
            var loaded = await TryLoadPropertiesAsync(pageUserName);
            return loaded && PermittedToChange;
        }
        public async Task<IActionResult> OnPostAcceptDescription(string newText, string name)
        {
            if (await isLoadedAndPermittedToChange(name))
            {
                PageUser.HomePage.Description = newText;
                context.UserPages.Update(PageUser.HomePage);
                await context.SaveChangesAsync();
                return new JsonResult(new ServerResponse(true, localizer["Description changed successfully"]));
            }
            return new JsonResult(new ServerResponse(false, localizer["DescriptionPermError"]));
        }
    }
}
