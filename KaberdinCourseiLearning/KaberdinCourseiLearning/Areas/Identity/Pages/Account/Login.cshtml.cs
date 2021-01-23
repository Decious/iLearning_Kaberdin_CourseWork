using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using KaberdinCourseiLearning.Areas.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using KaberdinCourseiLearning.Resources;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly ILogger<LoginModel> logger;
        private readonly IStringLocalizer<LoginModel> localizer;

        public LoginModel(SignInManager<CustomUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<CustomUser> userManager,
            IStringLocalizer<LoginModel> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(ResourceType =typeof(ValidationResource),Prompt = nameof(ValidationResource.NamePrompt), Name = nameof(ValidationResource.NamePrompt))]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(ValidationResource), Prompt = nameof(ValidationResource.Password), Name = nameof(ValidationResource.Password))]
            public string Password { get; set; }

            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.Remember_me_))]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, localizer["Invalid login attempt."]);
                    return Page();
                }
            }
            return Page();
        }
    }
}
