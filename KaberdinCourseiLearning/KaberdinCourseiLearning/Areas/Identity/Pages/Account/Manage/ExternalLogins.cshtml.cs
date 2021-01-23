using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Areas.Identity;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly IStringLocalizer<ExternalLoginsModel> localizer;

        public ExternalLoginsModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IStringLocalizer<ExternalLoginsModel> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID."]);
            }

            CurrentLogins = await userManager.GetLoginsAsync(user);
            OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID."]);
            }

            var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = localizer["The external login was not removed."];
                return RedirectToPage();
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = localizer["The external login was removed."];
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID."]);
            }

            var info = await signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new InvalidOperationException(localizer["Unexpected error occurred loading external login info for user with ID.",user.Id]);
            }

            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = localizer["The external login was not added. External logins can only be associated with one account."];
                return RedirectToPage();
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = localizer["The external login was added."];
            return RedirectToPage();
        }
    }
}
