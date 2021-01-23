using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly IStringLocalizer<SetPasswordModel> localizer;

        public SetPasswordModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IStringLocalizer<SetPasswordModel> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.PasswordRequired))]
            [StringLength(100, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.The_must_be_at_least_and_at_max_characters_long_), MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.Password), Prompt = nameof(ValidationResource.NewPassword))]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.PasswordConfirm), Prompt = nameof(ValidationResource.PasswordConfirm))]
            [Compare("NewPassword", ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.PasswordConfirmFailed))]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            var addPasswordResult = await userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = localizer["Your password has been set."];

            return RedirectToPage();
        }
    }
}
