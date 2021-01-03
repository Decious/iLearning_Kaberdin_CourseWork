using System;
using System.IO;
using System.Threading.Tasks;
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
        public string AvatarPath { get; set; }
        public bool PermittedToChange { get; set; }
        public ProfileModel(UserManager<CustomUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IdentityUser PageUser { get; set; }
        public async Task<IActionResult> OnGetAsync(string name)
        {
            if(name != null)
            {
                var user = await userManager.FindByNameAsync(name);
                if (user != null)
                {
                    PageUser = user;
                    AvatarPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "User", "Avatar", PageUser.Id + ".png");
                    var guest = await userManager.GetUserAsync(User);
                    PermittedToChange = await new UserValidator(userManager).isUserOwnerOrAdminAsync(guest, name);
                    return Page();
                }
            }
            return Redirect("~/Index");
        }
        public async Task<IActionResult> OnPostUploadAvatar(IFormFile file,string name)
        {
            var validator = new UserValidator(userManager);
            var user = await userManager.GetUserAsync(User);
            PageUser = await userManager.FindByNameAsync(name);
            if(await validator.isUserOwnerOrAdminAsync(user,name))
            {
                try
                {
                    if (file.Length > 0)
                    {
                        AvatarPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "User", "Avatar", PageUser.Id + ".png");
                        using (var stream = System.IO.File.Create(AvatarPath))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[AVATAR {user.UserName}]Exception during file upload.\n {e.Message}");
                }
                return new OkResult();
            }
            return Forbid();
        }
    }
}
