using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.User.Pages
{
    public class ProfileModel : PageModel
    {
        private CustomUserManager userManager;
        private ApplicationDbContext context;
        private ImageManager imageManager;
        private ProfileManager profileManager;
        public bool PermittedToChange { get; set; }
        public CustomUser PageUser { get; set; }
        public ProfileModel(CustomUserManager userManager, ApplicationDbContext context, ImageManager imageManager, ProfileManager profileManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.imageManager = imageManager;
            this.profileManager = profileManager;
        }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            if (name != null)
            {
                var loaded = await TryLoadPropertiesAsync(name);
                if (loaded)
                {
                    await LoadCustomUserReferencesAsync();
                    return Page();
                }
            }
            return Redirect("~/Index");
        }
        private async Task<bool> TryLoadPropertiesAsync(string pageUserName)
        {
            PageUser = await userManager.FindByNameAsync(pageUserName);
            bool result = PageUser != null;
            if (result)
            {
                PermittedToChange = await userManager.IsUserOwnerOrAdminAsync(User, pageUserName);
            }
            return result;
        }
        private async Task LoadCustomUserReferencesAsync()
        {
            await context.Entry(PageUser).Reference(i => i.HomePage).LoadAsync();
            await context.Entry(PageUser).Collection(i => i.ItemCollections).LoadAsync();
        }
        public async Task<IActionResult> OnPostUploadAvatar(IFormFile file, string name)
        {
            if (await isLoadedAndPermittedToChange(name))
            {
                _ = await imageManager.UploadAvatar(file, PageUser.Id);
                return new JsonResult(new ServerResponse(true, "Avatar was uploaded successfully"));
            }
            return new JsonResult(ServerResponse.MakeForbidden());
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
                await profileManager.ChangeDescriptionAsync(newText,PageUser.Id);
                return new JsonResult(new ServerResponse(true, "Description changed successfully"));
            }
            return new JsonResult(ServerResponse.MakeForbidden());
        }
    }
}
