using KaberdinCourseiLearning.Areas.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Pages
{
    [Authorize(Roles = "Administrator")]
    public class AdminPanelModel : PageModel
    {
        private SignInManager<CustomIdentity> signInManager;
        private UserManager<CustomIdentity> userManager;
        public AdminPanelModel(SignInManager<CustomIdentity> signInManager, UserManager<CustomIdentity> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public async Task<IActionResult> OnGet()
        {
            if (signInManager.IsSignedIn(User))
            {
                var currentUser = await userManager.GetUserAsync(User);
                if (!await isUserBannedOrRemovedAsync(currentUser))
                {
                    return Page();
                }
            }
            await SignOut();
            return RedirectToPage("Login");
        }
        private async Task<bool> isUserBannedOrRemovedAsync(CustomIdentity user)
        {
            return (user == null || await userManager.IsLockedOutAsync(user));
        }
        private async Task SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignOutAsync();
        }

    }
}