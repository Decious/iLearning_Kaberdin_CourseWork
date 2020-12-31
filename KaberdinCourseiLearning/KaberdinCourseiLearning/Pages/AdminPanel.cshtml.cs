using KaberdinCourseiLearning.Areas.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Pages
{
    [Authorize(Policy = "Admin")]
    public class AdminPanelModel : PageModel
    {
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        public AdminPanelModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [BindProperty]
        public String FormAction { get; set; }

        public void OnGet()
        {
        }
        private async Task<bool> isUserBannedOrRemovedAsync(IdentityUser user)
        {
            return (user == null || await userManager.IsLockedOutAsync(user));
        }
        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (await isUserBannedOrRemovedAsync(currentUser)) return Redirect("~/Index");
            var ids = Request.Form["Selected"];
            foreach (String id in ids)
            {
                var user = await userManager.FindByIdAsync(id);
                switch (FormAction)
                {
                    case "Block":
                        await userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                        await userManager.UpdateAsync(user);
                        break;
                    case "Unblock":
                        if (await userManager.GetLockoutEndDateAsync(user) > DateTime.Now)
                        {
                            await userManager.SetLockoutEndDateAsync(user, null);
                            await userManager.UpdateAsync(user);
                        }
                        break;
                    case "Delete":
                        _ = userManager.DeleteAsync(user);
                        break;
                }
            }
            return RedirectToPage();
        }

    }
}