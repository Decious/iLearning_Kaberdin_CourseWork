using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.User.Pages
{
    public class ProfileModel : PageModel
    {
        private UserManager<CustomUser> userManager;
        private ApplicationDbContext context;
        private CustomUser guestUser;
        private ImageManager imageManager;
        private CollectionManager collectionManager;
        private ProfileManager profileManager;
        public bool PermittedToChange { get; set; }
        public CustomUser PageUser { get; set; }
        public ProfileModel(UserManager<CustomUser> userManager, ApplicationDbContext context, ImageManager imageManager, CollectionManager collectionManager,ProfileManager profileManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.imageManager = imageManager;
            this.collectionManager = collectionManager;
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
            guestUser = await userManager.GetUserAsync(User);
            var result = (PageUser != null && guestUser != null);
            if (result)
            {
                PermittedToChange = await new UserValidator(userManager).IsUserOwnerOrAdminAsync(guestUser, pageUserName);
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
                return new OkResult();
            }
            return Forbid();
        }
        private async Task<bool> isLoadedAndPermittedToChange(string name)
        {
            var loaded = await TryLoadPropertiesAsync(name);
            return loaded && PermittedToChange;
        }
        public async Task<IActionResult> OnPostAcceptDescription(string newText, string name)
        {
            if (await isLoadedAndPermittedToChange(name))
            {
                await profileManager.ChangeDescriptionAsync(newText,PageUser.Id);
                return new OkResult();
            }
            return Forbid();
        }
        public async Task<IActionResult> OnPostDeleteCollection(int collectionID, string name)
        {
            if (await isLoadedAndPermittedToChange(name))
            {
                await collectionManager.DeleteCollectionAsync(collectionID);
                return new OkResult();
            }
            return Forbid();
        }
    }
}
