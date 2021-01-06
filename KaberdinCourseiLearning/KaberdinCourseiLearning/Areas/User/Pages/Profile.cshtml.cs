using System;
using System.IO;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.User.Pages
{
    public class ProfileModel : PageModel
    {
        private UserManager<CustomUser> userManager;
        private IWebHostEnvironment webHostEnvironment;
        private ApplicationDbContext context;
        private CustomUser guestUser;
        public string AvatarPath { get; set; }
        public string BGPath { get; set; }
        public bool PermittedToChange { get; set; }
        public CustomUser PageUser { get; set; }
        public ProfileModel(UserManager<CustomUser> userManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
            this.context = context;
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
            AvatarPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "User", "Avatar");
            BGPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "Collection", "Background");
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
            var loaded = await TryLoadPropertiesAsync(name);
            if (loaded)
            {
                if (PermittedToChange)
                {
                    await TrySaveFormFileAsync($"{AvatarPath}\\{PageUser.Id}.png", file);
                    return new OkResult();
                }
            }
            return Forbid();
        }
        private async Task TrySaveFormFileAsync(string path, IFormFile file)
        {
            try {
                if (file.Length > 0)
                {
                    using (var stream = System.IO.File.Create(path))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[AVATAR]Exception during file upload.\n {e.Message}");
            }
        }
        public async Task<IActionResult> OnPostAcceptDescription(string newText, string name)
        {
            var loaded = await TryLoadPropertiesAsync(name);
            if (loaded)
            {
                if (PermittedToChange)
                {
                    await ChangeDescriptionAsync(newText);
                    return new OkResult();
                }
            }
            return Forbid();
        }

        private async Task ChangeDescriptionAsync(string newText)
        {
            await context.Entry(PageUser).Reference(i => i.HomePage).LoadAsync();
            PageUser.HomePage.Description = newText;
            await userManager.UpdateAsync(PageUser);
        }
    }
}
