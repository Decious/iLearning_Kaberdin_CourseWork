using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace KaberdinCourseiLearning.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly IStringLocalizer<IndexModel> localizer;

        public IndexModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IStringLocalizer<IndexModel> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.NameRequired))]
            [StringLength(50, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = nameof(ValidationResource.The_must_be_at_least_and_at_max_characters_long_), MinimumLength = 4)]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.NamePrompt), Prompt = nameof(ValidationResource.NamePrompt))]
            public string Username { get; set; }
            [Phone(ErrorMessageResourceType = typeof(ValidationResource),ErrorMessageResourceName = nameof(ValidationResource.PhoneInvalid))]
            [Display(ResourceType = typeof(ValidationResource), Name = nameof(ValidationResource.PhonePrompt), Prompt = nameof(ValidationResource.PhonePrompt))]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(CustomUser user)
        {
            var userName = await userManager.GetUserNameAsync(user);
            var phoneNumber = await userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Username = userName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(localizer["Unable to load user with ID.", userManager.GetUserId(User)]);
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = localizer["Unexpected error when trying to set phone number."];
                    return RedirectToPage();
                }
            }
            var userName = await userManager.GetUserNameAsync(user);
            if (Input.Username != userName)
            {
                var setUserName = await userManager.SetUserNameAsync(user, Input.Username);
                if (!setUserName.Succeeded)
                {
                    StatusMessage = localizer["Unexpected error when trying to set Username."];
                    return RedirectToPage();
                }
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = localizer["Your profile has been updated"];
            return RedirectToPage();
        }
    }
}
