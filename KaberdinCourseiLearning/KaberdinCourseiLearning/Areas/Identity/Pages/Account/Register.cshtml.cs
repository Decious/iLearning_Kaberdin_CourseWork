using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CustomUser> signInManager;
        private readonly UserManager<CustomUser> userManager;

        public RegisterModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceType =typeof(ValidationResource),ErrorMessageResourceName =nameof(ValidationResource.NameRequired))]
            [StringLength(50, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.The_must_be_at_least_and_at_max_characters_long_), MinimumLength = 4)]
            [Display(ResourceType =typeof(ValidationResource),Name =nameof(ValidationResource.NamePrompt), Prompt = nameof(ValidationResource.NamePrompt))]
            public string UserName { get; set; }
            [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.EmailRequired))]
            [EmailAddress(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.EmailInvalid))]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.EmailPrompt), Prompt = nameof(ValidationResource.EmailPrompt))]
            public string Email { get; set; }

            [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.PasswordRequired))]
            [StringLength(100,ErrorMessageResourceType =typeof(ValidationResource),ErrorMessageResourceName =nameof(ValidationResource.The_must_be_at_least_and_at_max_characters_long_), MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.Password), Prompt = nameof(ValidationResource.Password))]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.PasswordConfirm), Prompt = nameof(ValidationResource.PasswordConfirm))]
            [Compare("Password", ErrorMessageResourceType = typeof(ValidationResource),ErrorMessageResourceName =nameof(ValidationResource.PasswordConfirmFailed))]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new CustomUser { UserName = Input.UserName, Email = Input.Email };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
